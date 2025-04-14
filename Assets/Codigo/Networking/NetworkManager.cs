using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using TMPro;
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
            Thread threadPublisher = new Thread(new ThreadStart(publishService));
            thread.Start();
            threadPublisher.Start();
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

        private void publishService()
        {
            Thread.Sleep(2000);
            IPAddress ip = getIP();
            Int32 port = httpServer.getPort();

            //Publico el dns_sd
            string ip_str =  "" + ip;
            string port_str = "" + port;
            AndroidJNI.AttachCurrentThread(); // Necesario 
            dns_sd = new DNS_SD(ip_str, port_str);
            string serviceName = dns_sd.StartAdvertising();
            AndroidJNI.DetachCurrentThread();
            if (toPrint != null) {
                Task task = dispatcher.EnqueueAsync(
                    serviceName,
                    (string serviceName) => dispatcher.updateServiceName(serviceName)
                );
                task.Wait();
            }
        }

        public void updateServiceName(string serviceName)
        {
            if (toPrint != null)
                toPrint.text = serviceName;
            
            if (Application.platform != RuntimePlatform.Android) {
                IPAddress ip = getIP();
                Int32 port = httpServer.getPort();
                Debug.Log("Listening in ip: " + ip + "  port: " + port);
            }
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
