using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Globalization;

public class MetricaDecrementoSonido : Metric
{
    private List<string> decrementos = new List<string>();

    void Start()
    {
        PublicarMetrica();
    }

    // Llamar desde MusicVolumeController cuando el volumen disminuye por inmovilidad
    public void OnVolumeDecrease()
    {
        string currentTime = DateTime.Now.ToString("T", new CultureInfo("es-ES"));
        decrementos.Add($"{currentTime}: Decremento de sonido por inmovilidad del usuario");
    }

    public override string Nombre
    {
        get { return "Metrica Decremento Sonido"; }
    }

    protected override void PublicarMetrica()
    {
        if (!metricaPublicada)
        {
            InvokeCondicionMetricaCumplida();
            metricaPublicada = true;
        }
    }

    public override void ReiniciarMetrica()
    {
        decrementos = new List<string>();
        ReiniciarPublicada();
    }

    protected override Modelo getModelo()
    {
        StringBuilder mensaje = new StringBuilder("");
        if (decrementos.Count > 0)
        {
            foreach (string evt in decrementos)
            {
                mensaje.Append($"{evt}\n");
            }
            // Quita el ultimo salto de linea
            mensaje = mensaje.Remove(mensaje.Length - 1, 1);
        }
        var modeloMetrica = new Modelo
        {
            Nombre = Nombre,
            NombreContenido = NombreContenido,
            Mensaje = mensaje.ToString(),
        };
        return modeloMetrica;
    }
} 