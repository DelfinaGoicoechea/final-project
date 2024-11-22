using System;
using System.Collections;
using System.Collections.Generic;
using SimpleHttpServer.Models;
using UnityEngine;
using DispatcherSpace;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
            // TODO: Crear una clase para cada ruta, asi pasa a estar mas organizado
            routes = new List<Models.Route>() {
                new Route {
                    Name = "Play Handler",
                    UrlRegex = @"^/Play$",
                    Method = "POST",
                    Callable = (HttpRequest request) => {
                        Func<bool, string, HttpResponse> callback = (bool isOk, string metricas) =>
                        {
                            if (isOk)
                            {
                                return new HttpResponse()
                                {
                                    ContentAsUTF8 = metricas,
                                    ReasonPhrase = "OK",
                                    StatusCode = "200"
                                };
                            }
                            else
                            {
                                return new HttpResponse()
                                {
                                    ContentAsUTF8 = metricas,
                                    ReasonPhrase = "Not Found",
                                    StatusCode = "404"
                                };
                            }
                        };

                        DataScene jsonObject = DataScene.CreateFromJSON(request.Content);
                        string scene = jsonObject.scene;

                        // Cargo posibles configuraciones de la escena
                        Dictionary<string, object> config;
                        if(!request.Headers.ContainsKey("config")){
                            config = new Dictionary<string, object>();
                        } else {
                            config = JsonConvert.DeserializeObject<Dictionary<string, object>>(request.Headers["config"]);
                        }

                        // Necesito usar un  task, porque play() se debe 
                        // ejecutar SI O SI desde el thread principal.
                        Task<HttpResponse> task = dispatcher.EnqueueAsync(
                            scene, config,
                            (string scene, Dictionary<string, object> config) =>
                            {
                                return dispatcher.play(scene, config, callback);
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
                new Route {
                    Name = "Customize Content Handler",
                    UrlRegex = @"^/CustomizeContent$",
                    Method = "POST",
                    Callable = (HttpRequest request) => {
                        Func<string, HttpResponse> callback = (string warning) =>
                        {
                            if (warning == "")
                            {
                                return new HttpResponse()
                                {
                                    ContentAsUTF8 = "Contenido modificado con exito",
                                    ReasonPhrase = "OK",
                                    StatusCode = "200"
                                };
                            }
                            else
                            {
                                return new HttpResponse()
                                {
                                    ContentAsUTF8 = warning,
                                    ReasonPhrase = "Not Found",
                                    StatusCode = "400"
                                };
                            }
                        };

                        Dictionary<string, object> cambios = JsonConvert.DeserializeObject<Dictionary<string, object>>(request.Content);

                        // Necesito usar un  task, porque customizeContent() se debe 
                        // ejecutar SI O SI desde el thread principal.
                        Task<HttpResponse> task = dispatcher.EnqueueAsync(
                            cambios,
                            (Dictionary<string, object> cambios) =>
                            {
                                return dispatcher.customizeContent(cambios, callback);
                            }
                        );
                        task.Wait();
                        return task.Result;
                    }
                },
                new Route {
                    Name = "get Config Handler",
                    UrlRegex = @"^/GetConfig$",
                    Method = "GET",
                    Callable = (HttpRequest request) => {
                        Func<string, HttpResponse> callback = (string config) =>
                        {
                            if(config != null){
                                return new HttpResponse()
                                {
                                    ContentAsUTF8 = config,
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
                            () => { return dispatcher.getCurrentContentConfig(scene, callback); }
                        );
                        task.Wait();
                        return task.Result;
                    }
                },
                new Route {
                    Name = "Delete Content Handler",
                    UrlRegex = @"^/Delete$",
                    Method = "POST",
                    Callable = (HttpRequest request) => {
                        Func<bool, HttpResponse> callback = (bool isOk) =>
                        {
                            if (isOk)
                            {
                                return new HttpResponse()
                                {
                                    ContentAsUTF8 = "El contenido se elimino correctamente",
                                    ReasonPhrase = "OK",
                                    StatusCode = "200"
                                };
                            }
                            else
                            {
                                return new HttpResponse()
                                {
                                    ContentAsUTF8 = "El contenido no pudo ser eliminado",
                                    ReasonPhrase = "Internal Server Error",
                                    StatusCode = "500"
                                };
                            }
                        };

                        DataScene jsonObject = DataScene.CreateFromJSON(request.Content);
                        string scene = jsonObject.scene;

                        // Necesito usar un  task, porque deleteContenido() se debe 
                        // ejecutar SI O SI desde el thread principal.
                        Task<HttpResponse> task = dispatcher.EnqueueAsync(
                            scene,
                            (string scene) =>
                            {
                                return dispatcher.deleteContenido(scene, callback);
                            }
                        );
                        task.Wait();
                        return task.Result;
                    }
                },
                new Route {
                    Name = "Add External App Handler",
                    UrlRegex = @"^/AddApp$",
                    Method = "POST",
                    Callable = (HttpRequest request) => {
                        Func<bool, HttpResponse> callback = (bool isOk) =>
                        {
                            if (isOk)
                            {
                                return new HttpResponse()
                                {
                                    ContentAsUTF8 = "Se agrego la app",
                                    ReasonPhrase = "OK",
                                    StatusCode = "200"
                                };
                            }
                            else
                            {
                                return new HttpResponse()
                                {
                                    ContentAsUTF8 = "Ya existe un contenido con el nombre inidcado",
                                    ReasonPhrase = "Bad Request",
                                    StatusCode = "400"
                                };
                            }
                        };

                        ExternalAppInfo jsonObject = ExternalAppInfo.CreateFromJSON(request.Content);
                        string visibleName = jsonObject.visibleName;
                        string packageName = jsonObject.packageName;

                        // Necesito usar un  task, porque addExternalApp() se debe 
                        // ejecutar SI O SI desde el thread principal.
                        Task<HttpResponse> task = dispatcher.EnqueueAsync(
                            visibleName, packageName,
                            (string visibleName, string packageName) =>
                            {
                                return dispatcher.addExternalApp(visibleName, packageName, callback);
                            }
                        );
                        task.Wait();
                        return task.Result;
                    }
                },
                new Route {
                    Name = "Add Photo Handler",
                    UrlRegex = @"^/AddPhoto$",
                    Method = "POST",
                    Callable = (HttpRequest request) => {
                        Func<bool, HttpResponse> callback = (bool isOk) =>
                        {
                            if (isOk)
                            {
                                return new HttpResponse()
                                {
                                    ContentAsUTF8 = "Se agrego la imagen",
                                    ReasonPhrase = "OK",
                                    StatusCode = "200"
                                };
                            }
                            else
                            {
                                return new HttpResponse()
                                {
                                    ContentAsUTF8 = "Ya existe un contenido con el nombre inidcado",
                                    ReasonPhrase = "Bad Request",
                                    StatusCode = "400"
                                };
                            }
                        };

                        FilePathInfo jsonObject = FilePathInfo.CreateFromJSON(request.Content);
                        string filepath = jsonObject.filepath;

                        // Necesito usar un  task, porque addExternalApp() se debe 
                        // ejecutar SI O SI desde el thread principal.
                        Task<HttpResponse> task = dispatcher.EnqueueAsync(
                            filepath,
                            (string filepath) =>
                            {
                                return dispatcher.addPhoto(filepath, callback);
                            }
                        );
                        task.Wait();
                        return task.Result;
                    }
                },
                new Route {
                    Name = "Add Video Handler",
                    UrlRegex = @"^/AddVideo$",
                    Method = "POST",
                    Callable = (HttpRequest request) => {
                        Func<bool, HttpResponse> callback = (bool isOk) =>
                        {
                            if (isOk)
                            {
                                return new HttpResponse()
                                {
                                    ContentAsUTF8 = "Se agrego el video",
                                    ReasonPhrase = "OK",
                                    StatusCode = "200"
                                };
                            }
                            else
                            {
                                return new HttpResponse()
                                {
                                    ContentAsUTF8 = "Ya existe un contenido con el nombre inidcado",
                                    ReasonPhrase = "Bad Request",
                                    StatusCode = "400"
                                };
                            }
                        };

                        FilePathInfo jsonObject = FilePathInfo.CreateFromJSON(request.Content);
                        string filepath = jsonObject.filepath;

                        // Necesito usar un  task, porque addExternalApp() se debe 
                        // ejecutar SI O SI desde el thread principal.
                        Task<HttpResponse> task = dispatcher.EnqueueAsync(
                            filepath,
                            (string filepath) =>
                            {
                                return dispatcher.addVideo(filepath, callback);
                            }
                        );
                        task.Wait();
                        return task.Result;
                    }
                },
                new Route {
                    Name = "Add Audio Handler",
                    UrlRegex = @"^/AddAudio$",
                    Method = "POST",
                    Callable = (HttpRequest request) => {
                        Func<bool, HttpResponse> callback = (bool isOk) =>
                        {
                            if (isOk)
                            {
                                return new HttpResponse()
                                {
                                    ContentAsUTF8 = "Se agrego el audio",
                                    ReasonPhrase = "OK",
                                    StatusCode = "200"
                                };
                            }
                            else
                            {
                                return new HttpResponse()
                                {
                                    ContentAsUTF8 = "Ya existe un audio con el nombre inidcado",
                                    ReasonPhrase = "Bad Request",
                                    StatusCode = "400"
                                };
                            }
                        };

                        FilePathInfo jsonObject = FilePathInfo.CreateFromJSON(request.Content);
                        string filepath = jsonObject.filepath;

                        // Necesito usar un  task, porque addExternalApp() se debe 
                        // ejecutar SI O SI desde el thread principal.
                        Task<HttpResponse> task = dispatcher.EnqueueAsync(
                            filepath,
                            (string filepath) =>
                            {
                                return dispatcher.addAudio(filepath, callback);
                            }
                        );
                        task.Wait();
                        return task.Result;
                    }
                },
                new Route {
                    // "Get Current Metrics
                    // Permite obtener las metricas de la escena actual sin llamar a play. 
                    // Es util para permitir reproduccion de la forma:
                    // 1. Photo, Video o Escena
                    // 2. ExternalApp
                    // de manera que desde Flutter antes de cambiar de aplicacion, pide las metricas de lo anterior, y
                    // ya no hay peligro de que se pierdan si android decide cerrar la app principal. 
                    Name = "Get Triggered Metrics Handler", 
                    UrlRegex = @"^/GetTriggeredMetrics$",
                    Method = "GET",
                    Callable = (HttpRequest request) => {
                        Func<string, HttpResponse> callback = (string metricas) =>
                        {
                            return new HttpResponse()
                            {
                                ContentAsUTF8 = metricas,
                                ReasonPhrase = "OK",
                                StatusCode = "200"
                            };
                        };

                        // Necesito usar un  task, porque getTriggeredMetrics() se debe 
                        // ejecutar SI O SI desde el thread principal.
                        Task<HttpResponse> task = dispatcher.EnqueueAsync(
                            () =>
                            {
                                return dispatcher.getTriggeredMetrics(callback);
                            }
                        );
                        task.Wait();
                        return task.Result;
                    }
                },
                new Route {
                    Name = "Check Connection Handler",
                    UrlRegex = @"^/CheckConnection$",
                    Method = "GET",
                    Callable = (HttpRequest request) => {
                        return new HttpResponse() {
                            ContentAsUTF8 = "",
                            ReasonPhrase = "OK",
                            StatusCode = "200"
                        };
                    }
                },
                new Route {
                    Name = "Get Last Update Time", 
                    UrlRegex = @"^/GetLastUpdateTime$",
                    Method = "GET",
                    Callable = (HttpRequest request) => {
                        Func<string, HttpResponse> callback = (string updateTime) =>
                        {
                            return new HttpResponse()
                            {
                                ContentAsUTF8 = updateTime,
                                ReasonPhrase = "OK",
                                StatusCode = "200"
                            };
                        };

                        // Necesito usar un  task, porque getLastUpdateTime() se debe 
                        // ejecutar SI O SI desde el thread principal.
                        Task<HttpResponse> task = dispatcher.EnqueueAsync(
                            () =>
                            {
                                return dispatcher.getLastUpdateTime(callback);
                            }
                        );
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