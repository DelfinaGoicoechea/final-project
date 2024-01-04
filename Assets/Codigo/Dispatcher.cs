using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        }

        public HttpResponse play(string scene, Func<bool, HttpResponse> callback)
        {
			// Levanta las metricas de la escena saliente (antes del cambio).
			mManager.LevantarMetricasEscena();
            bool sceneFound = cManager.playContent(scene);
            return callback(sceneFound);
        }

        public HttpResponse pause(Func<HttpResponse> callback)
        {
            StartCoroutine(cManager.pause());
            return callback();
        }

        public HttpResponse setVolume(float vol, Func<HttpResponse> callback)
        {
            cManager.setVolume(vol);
            return callback();
        }

        public HttpResponse stop(Func<string, HttpResponse> callback)
        {
			mManager.LevantarMetricasEscena();
            List<string> metricas = mManager.ReportarMetricas();
			DataMetricsList serializer = new DataMetricsList(metricas);
			string metricasJson = serializer.getAsJSON();
			StartCoroutine(cManager.stop());
            return callback(metricasJson);
        }
        
		
		public HttpResponse getContents(Func<string, HttpResponse> callback)
		{
			List<string> contenido = cManager.getContents();
			if(contenido.Count == 0)
				return null;
			StringBuilder json = new StringBuilder("{\"scenes\": [");
            foreach (string name in contenido)
            {
				json.Append($"\"{name}\",");
            }
            json = json.Remove(json.Length-1, 1);
            json.Append("]}");
			return callback(json.ToString());
		}

		// TODO: usar BASE64 y mandarlo en un json parece bastante horroroso, especialmente xq
		//       la biblioteca requiere que las imagenes no esten comprimidas.
		//       Una vez que se tenga la app Mobile, ver de pasar las imagenes en binario,
		//       o por el estilo.
		public HttpResponse getThumbnail(string scene, Func<string, HttpResponse> callback)
		{
			Texture2D thumbnail = cManager.getThumbnail(scene);
			string encodedThumbnail = null;
			if(thumbnail != null){
				encodedThumbnail = TextureToBase64Tool.Texture2DToBase64(thumbnail);
			}
			return callback(encodedThumbnail);
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

        // Enqueue functions with a SINGLE argument
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

        // Enqueue functions with a SINGLE argument
        // This is stupid, but I couldn't make a generic delegate function work :( 
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