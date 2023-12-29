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
using DispatcherSpace;
using DNSSpace;

namespace SimpleHttpServer
{
    public class NetworkManager : MonoBehaviour
    {

        public static NetworkManager Instance { get; private set; }
        public TMP_Text toPrint;
        private HttpServer httpServer;
        private Int32 port;

        public Dispatcher dispatcher;

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
            //Inicializo rutas
            Routes routes = new Routes();
            routes.setDispatcher(this.dispatcher);
            List<Models.Route> route_config  = routes.getRoutes();

            //Levanto servidor http
            httpServer = new HttpServer(route_config);
            Thread thread = new Thread(new ThreadStart(httpServer.Listen));
            thread.Start();
            StartCoroutine(showIpPort());

            //Publico el dns_sd
            DNS_SD dns_sd = new DNS_SD("Tranquilidad", port);
            dns_sd.StartAdvertising();
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
            port = httpServer.getPort();
            Debug.Log("Listening in ip: " + ip + "  port: " + port);
            if (toPrint != null)
                toPrint.text = "ip:" + ip + " port:" + port; // TODO: Eventually delete "toPrint", and this line too
        }
    }
}
