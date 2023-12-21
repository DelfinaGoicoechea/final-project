using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using SimpleHttpServer;
using SimpleHttpServer.Models;
using TMPro;


namespace SimpleHttpServer
{
    public class ServerLauncher : MonoBehaviour
    {
        private HttpServer httpServer;
        public TMP_Text toPrint;
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
                //new Route {   
                //    Name = "FileSystem Static Handler",
                //    UrlRegex = @"^/Static/(.*)$",
                //    Method = "GET",
                //    Callable = new FileSystemRouteHandler() { BasePath = @"C:\Tmp", ShowDirectories=true }.Handle,
                //},
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

        private IPAddress getIP() {
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

        private IEnumerator showIpPort(){
            yield return new WaitForSeconds(5);
            IPAddress ip = getIP();
            Int32 port = httpServer.getPort();
            Debug.Log("Listening in ip: " + ip + "  port: " + port);
            if (toPrint != null)
                toPrint.text = "ip:" + getIP() + " port:" + port; // TODO: Delete toPrint, and this line
        }
    }
}