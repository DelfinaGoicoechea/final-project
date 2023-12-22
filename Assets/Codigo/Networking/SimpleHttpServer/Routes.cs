using System;
using System.Collections;
using System.Collections.Generic;
using SimpleHttpServer.Models;
using UnityEngine;
//using Dispatcher, 

namespace SimpleHttpServer
{
    public class Routes
    {
        private List<Models.Route> routes;

        public Routes()
        {
            var routes = new List<Models.Route>() {
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
                        return tempTestResponse();
                        //return Dispatcher.play(scene, callback);
                        //Debug.Log(request.Content);
                    }
                },
            };
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
    }
}