using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using TMPro;
using SimpleHttpServer;
using SimpleHttpServer.Models;


namespace SimpleHttpServer
{
    public class NetworkManager : MonoBehaviour
    {

        public static NetworkManager Instance { get; private set; }
        public TMP_Text toPrint;
        private HttpServer httpServer;

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
            var route_config = new List<Models.Route>() {
                new Route {
                    Name = "Hello Handler",
                    UrlRegex = @"^/$",
                    Method = "GET",
                    Callable = (HttpRequest request) => {
                        return new HttpResponse()
                        {
                            ContentAsUTF8 = "Hello from SimpleHttpServer",
                            ReasonPhrase = "OK",
                            StatusCode = "200"
                        };
                    }
                },
                new Route {
                    Name = "Play Handler",
                    UrlRegex = @"^/Play$",
                    Method = "PUT",
                    Callable = (HttpRequest request) => {
                        // Aca debería tener un llamado ¿al dispatcher? dando play, y usando la
                        // escena indicada en request.Content
                        Debug.Log(request.Content);
                        return new HttpResponse()
                        {
                            ContentAsUTF8 = "the content is playing succensfully",
                            ReasonPhrase = "OK",
                            StatusCode = "200"
                        };
                    }
                },
            };
            httpServer = new HttpServer(route_config);
            Thread thread = new Thread(new ThreadStart(httpServer.Listen));
            thread.Start();
            StartCoroutine(showIpPort());
        }

        // Update is called once per frame
        void Update()
        {

        }

        private IPAddress getIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return IPAddress.Parse(ip.ToString());
                }
            }
            Debug.LogError("No se encontro la direccion IP");
            return null;
        }

        private IEnumerator showIpPort()
        {
            yield return new WaitForSeconds(5);
            IPAddress ip = getIP();
            Int32 port = httpServer.getPort();
            Debug.Log("Listening in ip: " + ip + "  port: " + port);
            if (toPrint != null)
                toPrint.text = "ip:" + ip + " port:" + port; // TODO: Eventually delete "toPrint", and this line too
        }
    }
}
