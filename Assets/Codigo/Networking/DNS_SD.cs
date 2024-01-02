using System;
using System.Threading.Tasks;
using Mono.Zeroconf.Providers.Bonjour;
using UnityEngine;

namespace DNSSpace
{
    public class DNS_SD
    {

        string IP;
        private string port;

        public async void StartAdvertising()
        {
            try
            {
                //src.DnsSdJava.ServiceRegistration
                Debug.Log("Todavia no crasheo 0");
                string[] args2 = new string[]{};
                AndroidJavaClass jc = new AndroidJavaClass("src.DnsSdJava.ServiceRegistration");
                Debug.Log("Todavia no crasheo 1");
                string[] args = new string[]{ IP, port};
                Debug.Log("Todavia no crasheo 2");
                jc.CallStatic("main", args);
                Debug.Log("Todavia no crasheo 3");
            }
            catch (Exception ex)
            {
                Debug.LogError("Error al iniciar el anuncio del servicio: " + ex.Message);
            }
        }

        public DNS_SD(String IP, String port)
        {
            this.IP = IP;
            this.port = port;
        }

    }
}