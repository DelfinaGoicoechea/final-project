using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleHttpServer;
using SimpleHttpServer.Models;
using Contents;
using System.Threading.Tasks;
using TeamUtilityEditor;
using System.Text;

namespace DispatcherSpace
{
    public class Dispatcher : MonoBehaviour
    {
        private static readonly Queue<Action> _executionQueue = new Queue<Action>();


        public static Dispatcher Instance { get; private set; }
        private ContentManager cManager;
        private MetricsManager mManager;
		private NetworkManager nManager;

        private void Awake()
        {
            // If there is an instance, and it's not me, delete myself.
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }
            DontDestroyOnLoad(this.gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {
            cManager = GameObject.FindWithTag("ContentManager").GetComponent<ContentManager>();
            mManager = GameObject.FindWithTag("MetricsManager").GetComponent<MetricsManager>();
			nManager = GameObject.FindWithTag("NetworkManager").GetComponent<NetworkManager>();
        }

		/// Funcion que realiza la accion de play y retorna la respuesta http acorde (codigo)
        public HttpResponse play(string scene, Func<bool, HttpResponse> callback)
        {	
			string currentlyPlaying = cManager.getPlayingName();
			// chequeo si ya hay contenido en reproduccion, 
			// y si es un cambio de contenido, o resumir una pausa
			if(currentlyPlaying != null && !currentlyPlaying.Equals(scene)){
				// Levanta las metricas de la escena saliente (antes del cambio)
				mManager.LevantarMetricasEscena();
			}
            bool sceneFound = cManager.playContent(scene);
            return callback(sceneFound);
        }

		/// <summary>
		/// Funcion que realiza la accion de pausa y retorna la respuesta http acorde (codigo)
		/// <summary>
		/// <returns></returns>
        public HttpResponse pause(Func<HttpResponse> callback)
        {
            StartCoroutine(cManager.pause());
            return callback();
        }

		/// <summary>
		/// Funcion que realiza la accion de cambio de volumen y retorna la respuesta http acorde (codigo)
		/// </summary>
		/// <returns></returns>
        public HttpResponse setVolume(float vol, Func<HttpResponse> callback)
        {
            cManager.setVolume(vol);
            return callback();
        }

		/// <summary>
		/// Funcion que realiza la accion de stop contenido y retorna la respuesta http acorde.
		/// Como se finaliza la sesion, le pido las metricas recogidas al Metrics Manager, las serializo y las devuelvo.
		/// </summary>
		/// <returns></returns>
        public HttpResponse stop(Func<string, HttpResponse> callback)
        {
			mManager.LevantarMetricasEscena();
            List<string> metricas = mManager.ReportarMetricas();
			DataMetricsList serializer = new DataMetricsList(metricas);
			string metricasJson = serializer.getAsJSON();
			StartCoroutine(cManager.stop());
            return callback(metricasJson);
        }
        
		///<summary>
		/// Funcion que pide al Content Manager todos los contenidos disponibles para reproduccion
		/// </summary>
		/// <param name="callback"></param>
		/// <returns></returns>		
		public HttpResponse getContents(Func<string, HttpResponse> callback)
		{
			Dictionary<string, List<string>> contenidos = cManager.getContents();
			if(contenidos.Count == 0)
				return null;
			StringBuilder json = new StringBuilder("{");
			bool had3d = false;
			bool hadVideos = false;
			bool hadPhotos = false;
            foreach (string contentType in contenidos.Keys)
            {
				switch (contentType){
					case "3D":
						had3d = true;
						if(hadVideos || hadPhotos){
							json.Append(",");
						}
						json.Append("\"3D\": [");
						foreach (string content in contenidos["3D"]){
							json.Append($"\"{content}\",");
						}
						json = json.Remove(json.Length-1, 1);
            			json.Append("]");
						break;
					
					case "videos":
						hadVideos = true;
						if(had3d || hadPhotos){
							json.Append(",");
						}
						json.Append("\"videos\": [");
						foreach (string content in contenidos["videos"]){
							json.Append($"\"{content}\",");
						}
						json = json.Remove(json.Length-1, 1);
            			json.Append("]");
						break;

					case "photos":
						hadPhotos = true;
						if(had3d || hadVideos){
							json.Append(",");
						}
						json.Append("\"photos\": [");
						foreach (string content in contenidos["photos"]){
							json.Append($"\"{content}\",");
						}
						json = json.Remove(json.Length-1, 1);
            			json.Append("]");
						break;
				}
            }
            json.Append("}");
			return callback(json.ToString());
		}

		// TODO: usar BASE64 y mandarlo en un json parece ineficiente, especialmente porque
		//       la biblioteca requiere que las imagenes no esten comprimidas.
		//       Posible mejora: pasar imagenes en binario u otra mejor opcion
		public HttpResponse getThumbnail(string scene, Func<string, HttpResponse> callback)
		{
			Texture2D thumbnail = cManager.getThumbnail(scene);
			string encodedThumbnail = null;
			if(thumbnail != null){
				encodedThumbnail = TextureToBase64Tool.Texture2DToBase64(thumbnail);
			}
			return callback(encodedThumbnail);
		}

		/// <summary>
		/// Funcion para dejar de publicar servicio mDNS. No usada.
		/// </summary>
		/// <param name="callback"></param>
		/// <returns></returns>		
		public HttpResponse stopAdvertising(Func<HttpResponse> callback){
			nManager.stopAdvertisingService();
			return callback();
		}

        // Update is called once per frame
		public void Update() {
			lock(_executionQueue) {
				while (_executionQueue.Count > 0) {
					_executionQueue.Dequeue().Invoke();
				}
			}
		}

        public void Enqueue(IEnumerator action) {
			lock (_executionQueue) {
				_executionQueue.Enqueue (() => {
					StartCoroutine (action);
				});
			}
		}

        /// <summary>
		/// Enqueue functions with a SINGLE argument
		/// </summary>
		/// <param name="argument"></param>
		/// <param name="action"></param>
		/// <returns></returns>		
        public Task<HttpResponse> EnqueueAsync(string argument, Func<string, HttpResponse> action)
		{
			var tcs = new TaskCompletionSource<HttpResponse>();

			void WrappedAction() {
				try 
				{
					HttpResponse response = action(argument);
					tcs.TrySetResult(response);
				} catch (Exception ex) 
				{
					tcs.TrySetException(ex);
				}
			}

			Enqueue(ActionWrapper(WrappedAction));
			return tcs.Task;
		}

        /// <summary>
		/// Enqueue functions with a SINGLE argument
		/// </summary>
		/// <param name="argument"></param>
		/// <param name="action"></param>
		/// <returns></returns>		
        // This is ineficient, but I couldn't make a generic delegate function work :( 
        public Task<HttpResponse> EnqueueAsync(float argument, Func<float, HttpResponse> action)
		{
			var tcs = new TaskCompletionSource<HttpResponse>();

			void WrappedAction() {
				try 
				{
					HttpResponse response = action(argument);
					tcs.TrySetResult(response);
				} catch (Exception ex) 
				{
					tcs.TrySetException(ex);
				}
			}

			Enqueue(ActionWrapper(WrappedAction));
			return tcs.Task;
		}

        // Enqueue functions with NO arguments
        public Task<HttpResponse> EnqueueAsync(Func<HttpResponse> action)
		{
			var tcs = new TaskCompletionSource<HttpResponse>();

			void WrappedAction() {
				try 
				{
					HttpResponse response = action();
					tcs.TrySetResult(response);
				} catch (Exception ex) 
				{
					tcs.TrySetException(ex);
				}
			}

			Enqueue(ActionWrapper(WrappedAction));
			return tcs.Task;
		}

        IEnumerator ActionWrapper(Action a)
		{
			a();
			yield return null;
		}
    }
}