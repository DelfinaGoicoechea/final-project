using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
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
                        Func<bool, HttpResponse> callback = (bool isOk) =>
                        {
                            if (isOk)
                            {
                                return new HttpResponse()
                                {
                                    ContentAsUTF8 = "El contenido comenzo a reproducirse",
                                    ReasonPhrase = "OK",
                                    StatusCode = "200"
                                };
                            }
                            else
                            {
                                return new HttpResponse()
                                {
                                    ContentAsUTF8 = "El contenido solicitado no fue encontrado",
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
                        // No estoy seguro de si el siguiente Wait() espera
                        // a que se termine de ejecutar el task 
                        // TODO: confirmar eso.
                        task.Wait();
                        return task.Result;
                    }
                },
                new Route {
                    Name = "Pause Handler",
                    UrlRegex = @"^/Pause$",
                    Method = "PUT",
                    Callable = (HttpRequest request) => {
                        Func<HttpResponse> callback = () =>
                        {
                            return new HttpResponse()
                            {
                                ContentAsUTF8 = "La reproducion fue pausada",
                                ReasonPhrase = "OK",
                                StatusCode = "200"
                            };
                        };
                        // Necesito usar un  task, porque pause() se debe 
                        // ejecutar SI O SI desde el thread principal.
                        Task<HttpResponse> task = dispatcher.EnqueueAsync(
                            () => { return dispatcher.pause(callback); }
                        );
                        // No estoy seguro de si el siguiente Wait() espera
                        // a que se termine de ejecutar el task 
                        // TODO: confirmar eso.
                        task.Wait();
                        return task.Result;
                    }
                },
                new Route {
                    Name = "Stop Handler",
                    UrlRegex = @"^/Stop$",
                    Method = "PUT",
                    Callable = (HttpRequest request) => {
                        Func<string, HttpResponse> callback = (string metricas) =>
                        {
                            return new HttpResponse()
                            {
                                ContentAsUTF8 = metricas,
                                ReasonPhrase = "OK",
                                StatusCode = "200",
                            };
                        };
                        // Necesito usar un  task, porque stop() se debe 
                        // ejecutar SI O SI desde el thread principal.
                        Task<HttpResponse> task = dispatcher.EnqueueAsync(
                            () => { return dispatcher.stop(callback); }
                        );
                        // No estoy seguro de si el siguiente Wait() espera
                        // a que se termine de ejecutar el task 
                        // TODO: confirmar eso.
                        task.Wait();
                        return task.Result;
                    }
                },
                new Route {
                    Name = "Volume Handler",
                    UrlRegex = @"^/Volume$",
                    Method = "PUT",
                    Callable = (HttpRequest request) => {
                        Func<HttpResponse> callback = () =>
                        {
                            return new HttpResponse()
                            {
                                ContentAsUTF8 = "El volumen fue actualizado",
                                ReasonPhrase = "OK",
                                StatusCode = "200"
                            };
                        };

                        // obtengo el json, y de el su volumen
                        DataVolumen jsonObject = DataVolumen.CreateFromJSON(request.Content);
                        float volume = jsonObject.volumen;

                        // Necesito usar un  task, porque setVolumen() se debe 
                        // ejecutar SI O SI desde el thread principal.
                        Task<HttpResponse> task = dispatcher.EnqueueAsync(
                            () => { return dispatcher.setVolume(volume,callback); }
                        );
                        // No estoy seguro de si el siguiente Wait() espera
                        // a que se termine de ejecutar el task 
                        // TODO: confirmar eso.
                        task.Wait();
                        return task.Result;
                    }
                },
            };
        }

        public List<Models.Route> getRoutes()
        {
            // TODO: retornar una copia profunda, no la lista directa (ni copia superficial).
            return routes;
        }

        public void setDispatcher(Dispatcher disp)
        {
            this.dispatcher = disp;
        }
    }
}