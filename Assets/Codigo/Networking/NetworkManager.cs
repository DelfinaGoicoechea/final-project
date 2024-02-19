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
    /// <summary>
    /// Clase de gestion de Networking. Tiene al servidor http y DNS-SD como subrutinas.
    /// Es SINGLETON y no muere en toda la ejecucion.
    /// </summary>
    public class NetworkManager : MonoBehaviour
    {

        public static NetworkManager Instance { get; private set; }
        public TMP_Text toPrint;
        private HttpServer httpServer;
        private Int32 port;

        private DNS_SD dns_sd;

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
            StartCoroutine(publishService());
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

        private IEnumerator publishService()
        {
            yield return new WaitForSeconds(2);
            IPAddress ip = getIP();
            port = httpServer.getPort();
            Debug.Log("Listening in ip: " + ip + "  port: " + port);
            if (toPrint != null)
                toPrint.text = "ip:" + ip + " port:" + port; // TODO: Eventually delete "toPrint", and this line too
            
            //Publico el dns_sd
            string ip_str =  "" + ip;
            string port_str = "" + port;
            dns_sd = new DNS_SD(ip_str, port_str);
            dns_sd.StartAdvertising();
        }

        public void stopAdvertisingService()
        {
            dns_sd.StopAdvertising();
        }

        void OnDestroy(){
            //httpServer.Close();
        }
    }
}
