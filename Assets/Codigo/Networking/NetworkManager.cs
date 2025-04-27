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
using System.IO;

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
        public string serviceName;

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
            // Guardo el ip y el puerto en archivo, como mitigacion a problemas de DNS-SD en Windows
            string ipPortPath = $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}ip_port.txt";

            //Inicializo rutas
            Routes routes = new Routes();
            routes.setDispatcher(this.dispatcher);
            List<Models.Route> route_config  = routes.getRoutes();

            //Levanto servidor http
            httpServer = new HttpServer(route_config);
            Thread thread = new Thread(new ThreadStart(httpServer.Listen));
            Thread threadPublisher = new Thread(()=>publishService(ipPortPath));
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

        private void publishService(string ipPortPath)
        {
            Thread.Sleep(2000);
            string ip_str = "" + getIP();
            string port_str = "" + httpServer.getPort();
            
            // Guardo el ip y el puerto en archivo, como mitigacion a problemas de DNS-SD en Windows
            File.WriteAllText(ipPortPath, ip_str + ":" + port_str);

            //Publico el dns_sd
            AndroidJNI.AttachCurrentThread(); // Necesario por estar en thread secundario
            dns_sd = new DNS_SD(ip_str, port_str);
            string serviceName = dns_sd.StartAdvertising();
            AndroidJNI.DetachCurrentThread(); // Necesario por estar en thread secundario
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
            this.serviceName = serviceName;
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
