using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zeroconf;

public class DNS_SD : MonoBehaviour
{
    private IServicePublisher publisher;

    public async void StartAdvertising(string serviceName, int port)
    {
        try
        {
            publisher = new ServicePublisher();

            var properties = new[]
            {
                //Se puden crear tuplas de propiedades de la siguiente forma
                //Tuple.Create("key1", "value1"),
                //Tuple.Create("key2", "value2")
            };

            await publisher.PublishAsync(serviceName, "_http._tcp", port, properties);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al iniciar el anuncio del servicio: " + ex.Message);
        }
    }

    public void StopAdvertising()
    {
        publisher?.Unpublish();
    }
}
