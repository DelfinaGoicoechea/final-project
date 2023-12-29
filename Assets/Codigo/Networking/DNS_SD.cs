using System;
using System.Threading.Tasks;
using Mono.Zeroconf.Providers.Bonjour;
using UnityEngine;

namespace DNSSpace
{
    public class DNS_SD
    {
        //private IServicePublisher publisher;
        private string serviceName;
        private Int32 port;

        public async void StartAdvertising()
        {
            try
            {
                var properties = new TxtRecord{// Add properties if necessary
                };
                properties.Add ("Port", $"{port}");
                var service = new RegisterService
                {
                    Name = serviceName,
                    RegType = "_http._tcp",
                    ReplyDomain = "local.",
                    Port = Convert.ToInt16(5353),
                    TxtRecord = properties
                };

                Console.WriteLine("*** Registering name = '{0}', type = '{1}', domain = '{2}'",
                service.Name,
                service.RegType,
                service.ReplyDomain);

                service.Register();
                Debug.Log("Se registro el servicio en dns-sd");
                //publisher = new ServicePublisher();
                //await publisher.PublishAsync(service);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error al iniciar el anuncio del servicio: " + ex.Message);
            }
        }

        private static void OnServiceAdded(object o, ServiceBrowseEventArgs args)
        {
            Console.WriteLine("*** Found name = '{0}', type = '{1}', domain = '{2}'",
                args.Service.Name,
                args.Service.RegType,
                args.Service.ReplyDomain);
        }

        private static void OnServiceRemoved(object o, ServiceBrowseEventArgs args)
        {
            Console.WriteLine("*** Lost  name = '{0}', type = '{1}', domain = '{2}'",
                args.Service.Name,
                args.Service.RegType,
                args.Service.ReplyDomain);
        }

        public DNS_SD(string serviceName, Int32 port)
        {
            this.serviceName = serviceName;
            this.port = port;
        }

        /*public void StopAdvertising()
        {
            publisher?.Unpublish();
        }*/
    }
}
