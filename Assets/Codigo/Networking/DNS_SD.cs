using System;
using UnityEngine;

namespace DNSSpace
{
    /// <summary>
    /// Clase encargada del Service Discovery. Utiliza .jar ubicado en Plugins.
    /// </summary>
    public class DNS_SD
    {
        string IP;
        private string port;
        AndroidJavaClass jc = new AndroidJavaClass("src.DnsSdJava.ServiceRegistration");

        public string StartAdvertising()
        {
            // Si estoy en Windows ya retorno, porque (por ahora) la publicacion del servicio
            // solo funciona en Android
            if (System.Environment.OSVersion.Platform ==  PlatformID.Win32NT) {
                return "Tranquiliza.360";
            }
            
            string[] args = new string[]{ IP, port};

            // Hay 3 casos del metodo CallStatic (aunque la documentacion liste 2):
            //    1. public void CallStatic(string methodName, params object[] args)
            //    2. public ReturnType CallStatic(string methodName, params object[] args)
            //    3. public ReturnType CallStatic(string methodName, params T[] args)
            try
            {
                // uso <string,string> para darle los tipos al caso "3."
                string registeredName = jc.CallStatic<string,string>("registerService", args);
                if (registeredName.Contains("Tranquilidad 360"))
                {
                    registeredName = registeredName.Replace("Tranquilidad 360", "Tranquiliza.360");
                }
                return registeredName;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

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