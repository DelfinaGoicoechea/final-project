using System;
using System.Collections;
using System.Collections.Generic;
using SimpleHttpServer.Models;
using UnityEngine;
using DispatcherSpace;

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
                        // return tempTestResponse();
                        string scene = "mar2";
                        // TODO: Ver como llamar a hacer que dispatcher llame a play con (scene,callback),
                        // esperar y obtener lo que retorna.
                        dispatcher.EnqueueAsync(() =>
                        {
                            HttpResponse response = dispatcher.play(scene, callback);
                            Debug.Log("This is a debug log called on the main thread!");
                        });

                        return dispatcher.play(scene, callback);
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