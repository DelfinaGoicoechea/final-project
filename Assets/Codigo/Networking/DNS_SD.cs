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
        AndroidJavaClass jc = new AndroidJavaClass("src.DnsSdJava.ServiceRegistration");

        public async void StartAdvertising()
        {
            try
            {
                string[] args = new string[]{ IP, port};
                jc.CallStatic("registerService", args);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error al iniciar el anuncio del servicio: " + ex.Message);
            }
        }

        //TODO: Testear StopAdvertising en el propio Quest
        public void StopAdvertising()
        {
            try
            {
                jc.CallStatic("unregisterAllServices");
            }
            catch (Exception ex)
            {
                Debug.LogError("Error al finalizar el anuncio del servicio: " + ex.Message);
            }
        }

        public DNS_SD(String IP, String port)
        {
            this.IP = IP;
            this.port = port;
        }

    }
}