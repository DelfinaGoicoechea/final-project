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
			bool hadWalks = false;
            foreach (string contentType in contenidos.Keys)
            {
				switch (contentType){
					case "3D":
						had3d = true;
						if(hadVideos || hadPhotos || hadWalks){
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
						if(had3d || hadPhotos || hadWalks){
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
						if(had3d || hadVideos || hadWalks){
							json.Append(",");
						}
						json.Append("\"photos\": [");
						foreach (string content in contenidos["photos"]){
							json.Append($"\"{content}\",");
						}
						json = json.Remove(json.Length-1, 1);
            			json.Append("]");
						break;

					case "walks":
						hadWalks = true;
						if (hadPhotos || hadVideos || had3d){
							json.Append(",");
						}
						json.Append("\"walks\": [");
						foreach (string content in contenidos["walks"]){
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

		public HttpResponse getAudios(Func<string, HttpResponse> callback)
		{
			List<string> availableAudios = cManager.getAudios();
			StringBuilder json = new StringBuilder("{\"audios\": [");
			foreach (string audio in availableAudios)
			{
				json.Append($"\"{audio}\",");
			}
			json = json.Remove(json.Length-1, 1);
			json.Append("]}");
			return callback(json.ToString());
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

		public HttpResponse customizePhoto(string oldName, string newName, string newAudio, Func<string, HttpResponse> callback){
			string resultado = cManager.customizePhoto(oldName, newName, newAudio);
			return callback(resultado);
		}

		public HttpResponse customizeVideo(string oldName, bool isEquiangular, bool changedRepresentation, string newName, string newAudio, Func<string, HttpResponse> callback){
			string resultado = cManager.customizeVideo(oldName, isEquiangular, changedRepresentation, newName, newAudio);
			return callback(resultado);
		}

		public HttpResponse customizeEscena(string oldName, string newName, string newAudio, Func<string, HttpResponse> callback){
			string resultado = cManager.customizeEscena(oldName, newName, newAudio);
			return callback(resultado);
		}

		public HttpResponse customizeExtApp(string oldName, string newName, string packageName, Func<string, HttpResponse> callback){
			string resultado = cManager.customizeExtApp(oldName, newName, packageName);
			return callback(resultado);
		}

		public HttpResponse deleteContenido(string scene, Func<bool, HttpResponse> callback)
        {	
			string currentlyPlaying = cManager.getPlayingName();
			// chequeo si el contenido esta en reproduccion
			if(currentlyPlaying != null && !currentlyPlaying.Equals(scene)){
				// Levanta las metricas de la escena saliente (antes de la eliminacion)
				mManager.LevantarMetricasEscena();
			}
            bool wasSuccesfull = cManager.deleteContent(scene);
            return callback(wasSuccesfull);
        }

		public HttpResponse addExternalApp(string visibleName, string packageName, Func<bool, HttpResponse> callback){
			bool resultado = cManager.addExternalApp(visibleName, packageName);
			return callback(resultado);
		}

		public HttpResponse getContentConfig(string name, Func<string, HttpResponse> callback){
			Dictionary<string,string> config = cManager.getContentConfig(name);
			// Si el contenido no existe, se retorna null
			if (config == null){
				return callback(null);
			}

			StringBuilder jsonConfig = new StringBuilder("{");
			foreach (string key in config.Keys)
			{
				jsonConfig.Append($"\"{key}\":");
				// isEAC era un boolean, asi que lo trato distinto
				if(key == "isEAC"){
					jsonConfig.Append(config[key]+",");
				}else{
					jsonConfig.Append($"\""+config[key]+"\",");
				}
			}
			jsonConfig = jsonConfig.Remove(jsonConfig.Length-1, 1);
			jsonConfig.Append("}");
			return callback(jsonConfig.ToString());
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
		/// Enqueue functions with Three strings and a boolean as arguments
		/// </summary>
		/// <param name="argument"></param>
		/// <param name="action"></param>
		/// <returns></returns>		
        public Task<HttpResponse> EnqueueAsync(string arg1, bool arg2, bool arg3,string arg4, string arg5,  Func<string, bool, bool, string, string, HttpResponse> action)
		{
			var tcs = new TaskCompletionSource<HttpResponse>();

			void WrappedAction() {
				try 
				{
					HttpResponse response = action(arg1, arg2, arg3, arg4, arg5);
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
		/// Enqueue functions with Three string arguments
		/// </summary>
		/// <param name="argument"></param>
		/// <param name="action"></param>
		/// <returns></returns>		
        public Task<HttpResponse> EnqueueAsync(string arg1,string arg2, string arg3, Func<string, string, string, HttpResponse> action)
		{
			var tcs = new TaskCompletionSource<HttpResponse>();

			void WrappedAction() {
				try 
				{
					HttpResponse response = action(arg1, arg2, arg3);
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
		/// Enqueue functions with Two string arguments
		/// </summary>
		/// <param name="argument"></param>
		/// <param name="action"></param>
		/// <returns></returns>		
        public Task<HttpResponse> EnqueueAsync(string arg1,string arg2, Func<string, string, HttpResponse> action)
		{
			var tcs = new TaskCompletionSource<HttpResponse>();

			void WrappedAction() {
				try 
				{
					HttpResponse response = action(arg1, arg2);
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