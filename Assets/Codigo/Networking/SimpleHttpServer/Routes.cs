using System;
using System.Collections;
using System.Collections.Generic;
using SimpleHttpServer.Models;
using UnityEngine;
using DispatcherSpace;
using System.Threading.Tasks;

namespace SimpleHttpServer
{
    public class Routes
    {
        private List<Models.Route> routes;
        private Dispatcher dispatcher;

        public Routes()
        {
            routes = new List<Models.Route>() {
                new Route {
                    Name = "Play Handler",
                    UrlRegex = @"^/Play$",
                    Method = "PUT",
                    Callable = (HttpRequest request) => {
                        // Aca debería tener un llamado ¿al dispatcher? dando play, y usando la
                        // escena indicada en request.Content
                        Func<bool, HttpResponse> callback = (bool isOk) =>{
                            if (isOk)
                            {
                                return new HttpResponse()
                                {
                                    ContentAsUTF8 = "the content is playing succensfully",
                                    ReasonPhrase = "OK",
                                    StatusCode = "200"
                                };
                            }
                            else
                            {
                                return new HttpResponse()
                                {
                                    ContentAsUTF8 = "the requested content is not found",
                                    ReasonPhrase = "Not Found",
                                    StatusCode = "404"
                                };
                            }
                        };
                        // TODO: obtener la escena desde el request
                        string scene = "mar2";
                        // Necesito usar un  task, porque play() se debe 
                        // ejecutar SI O SI desde el thread principal.
                        Task<HttpResponse> task = dispatcher.EnqueueAsync(
                            scene, 
                            (string scene) =>
                            {
                                return dispatcher.play(scene, callback);
                            }
                        );
                        // No estoy seguro de si el siguiente return espera
                        // a que se termine de ejecutar el task 
                        // TODO: confirmar eso.
                        return task.Result;
                    }
                },
            };
        }

        public List<Models.Route> getRoutes(){
            // TODO: retornar una copia profunda, no la lista directa (ni copia superficial).
            return routes;
        }

        private HttpResponse tempTestResponse()
        {
            return new HttpResponse()
            {
                ContentAsUTF8 = "this is a testResponse",
                ReasonPhrase = "OK",
                StatusCode = "200"
            };
        }

        public void setDispatcher(Dispatcher disp){
            this.dispatcher = disp;
        }
    }
}