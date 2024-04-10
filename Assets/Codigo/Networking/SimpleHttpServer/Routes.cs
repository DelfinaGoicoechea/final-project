using System;
using System.Collections;
using System.Collections.Generic;
using SimpleHttpServer.Models;
using UnityEngine;
using DispatcherSpace;
using System.Threading.Tasks;

namespace SimpleHttpServer
{
    /// <summary>
    /// Establece las rutas para cada uno de los mensajes HTTP y la respuesta ante cada caso.
    /// </summary>
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
                    Method = "POST",
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

                        DataScene jsonObject = DataScene.CreateFromJSON(request.Content);
                        string scene = jsonObject.scene;

                        // Necesito usar un  task, porque play() se debe 
                        // ejecutar SI O SI desde el thread principal.
                        Task<HttpResponse> task = dispatcher.EnqueueAsync(
                            scene,
                            (string scene) =>
                            {
                                return dispatcher.play(scene, callback);
                            }
                        );
                        task.Wait();
                        return task.Result;
                    }
                },
                new Route {
                    Name = "Pause Handler",
                    UrlRegex = @"^/Pause$",
                    Method = "POST",
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
                        task.Wait();
                        return task.Result;
                    }
                },
                new Route {
                    Name = "Stop Handler",
                    UrlRegex = @"^/Stop$",
                    Method = "POST",
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
                        task.Wait();
                        return task.Result;
                    }
                },
                new Route {
                    Name = "Volume Handler",
                    UrlRegex = @"^/Volume$",
                    Method = "POST",
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

                        // Obtengo el json, y de el su volumen
                        DataVolumen jsonObject = DataVolumen.CreateFromJSON(request.Content);
                        float volume = jsonObject.volumen;

                        // Necesito usar un  task, porque setVolumen() se debe 
                        // ejecutar SI O SI desde el thread principal.
                        Task<HttpResponse> task = dispatcher.EnqueueAsync(
                            () => { return dispatcher.setVolume(volume,callback); }
                        );
                        task.Wait();
                        return task.Result;
                    }
                },
                new Route {
                    Name = "get Contents Handler",
                    UrlRegex = @"^/GetContents$",
                    Method = "GET",
                    Callable = (HttpRequest request) => {
                        Func<string, HttpResponse> callback = (string content) =>
                        {
                            if(content != null){
                                return new HttpResponse()
                                {
                                    ContentAsUTF8 = content,
                                    ReasonPhrase = "OK",
                                    StatusCode = "200"
                                };
                            }
                            else
                            {
                                return new HttpResponse()
                                {
                                    ContentAsUTF8 = "No se encontro contenido",
                                    ReasonPhrase = "Not Found",
                                    StatusCode = "404"
                                };
                            }
                        };

                        // Necesito usar un  task, porque setVolumen() se debe 
                        // ejecutar SI O SI desde el thread principal.
                        Task<HttpResponse> task = dispatcher.EnqueueAsync(
                            () => { return dispatcher.getContents(callback); }
                        );
                        task.Wait();
                        return task.Result;
                    }
                },
                new Route {
                    Name = "get Thumbnail Handler",
                    UrlRegex = @"^/GetThumbnail$",
                    Method = "GET",
                    Callable = (HttpRequest request) => {
                        Func<string, HttpResponse> callback = (string thumbnail) =>
                        {
                            if(thumbnail != null){
                                return new HttpResponse()
                                {
                                    ContentAsUTF8 = thumbnail,
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

                        if(!request.Headers.ContainsKey("scene")){
                            return new HttpResponse()
                            {
                                ContentAsUTF8 = "No se indico la escena a obtener",
                                ReasonPhrase = "Not Found",
                                StatusCode = "404"
                            };
                            
                        }

                        string scene = request.Headers["scene"];

                        // Necesito usar un  task, porque setVolumen() se debe 
                        // ejecutar SI O SI desde el thread principal.
                        Task<HttpResponse> task = dispatcher.EnqueueAsync(
                            () => { return dispatcher.getThumbnail(scene, callback); }
                        );
                        task.Wait();
                        return task.Result;
                    }
                },
                new Route {
                    Name = "get Audios Handler",
                    UrlRegex = @"^/GetAudios$",
                    Method = "GET",
                    Callable = (HttpRequest request) => {
                        Func<string, HttpResponse> callback = (string audios) =>
                        {
                            if(audios != null)
                            {
                                return new HttpResponse()
                                {
                                    ContentAsUTF8 = audios,
                                    ReasonPhrase = "OK",
                                    StatusCode = "200"
                                };
                            }
                            else
                            {
                                return new HttpResponse()
                                {
                                    ContentAsUTF8 = "No se encontro audios",
                                    ReasonPhrase = "Not Found",
                                    StatusCode = "404"
                                };
                            }
                        };

                        // Necesito usar un  task, porque setVolumen() se debe 
                        // ejecutar SI O SI desde el thread principal.
                        Task<HttpResponse> task = dispatcher.EnqueueAsync(
                            () => { return dispatcher.getAudios(callback); }
                        );
                        task.Wait();
                        return task.Result;
                    }
                },
                new Route {
                    Name = "Stop Advertisment Handler",
                    UrlRegex = @"^/RegisterClient$",
                    Method = "POST",
                    Callable = (HttpRequest request) => {
                        Func<HttpResponse> callback = () =>
                        {
                            return new HttpResponse()
                            {
                                ContentAsUTF8 = "El dispositivo fue registrado",
                                ReasonPhrase = "OK",
                                StatusCode = "200"
                            };
                        };

                        // Necesito usar un  task, porque setVolumen() se debe 
                        // ejecutar SI O SI desde el thread principal.
                        Task<HttpResponse> task = dispatcher.EnqueueAsync(
                            () => { return dispatcher.stopAdvertising(callback); }
                        );
                        task.Wait();
                        return task.Result;
                        //return dispatcher.stopAdvertising(callback);
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