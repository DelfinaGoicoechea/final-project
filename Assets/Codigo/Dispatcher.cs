using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleHttpServer.Models;
using Contents;
using System.Threading.Tasks;

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
            //mManager = GameObject.FindWithTag("MetricsManager");
        }

        public HttpResponse play(string scene, Func<bool, HttpResponse> callback)
        {
            bool sceneFound = cManager.playContent(scene);
            return callback(sceneFound);
        }

        public HttpResponse pause(Func<HttpResponse> callback)
        {
            cManager.pause();
            return callback();
        }

        public HttpResponse setVolume(float vol, Func<HttpResponse> callback)
        {
            cManager.setVolume(vol);
            return callback();
        }

        public HttpResponse stop(Func<HttpResponse> callback)
        {
            cManager.stop();
            List<string> metricas = mManager.ReportarMetricas();
            // TODO: Formatear lista de metricas en el callback. 
            // (lo que obtengo de ReportarMetricas() es una lista de jsons)
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