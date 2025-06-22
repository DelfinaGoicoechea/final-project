using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Globalization;

public class MetricaDuracionFlorAbierta : Metric
{
    private List<string> duraciones = new List<string>();
    private DateTime? openTimestamp = null;

    void Start()
    {
        PublicarMetrica();
    }

    // Llamar cuando la flor se abre
    public void OnFlowerOpened()
    {
        if (openTimestamp == null)
        {
            openTimestamp = DateTime.Now;
        }
    }

    // Llamar cuando la flor se cierra
    public void OnFlowerClosed()
    {
        if (openTimestamp != null)
        {
            DateTime closeTimestamp = DateTime.Now;
            TimeSpan duration = closeTimestamp - openTimestamp.Value;
            string openStr = openTimestamp.Value.ToString("T", new CultureInfo("es-ES"));
            string closeStr = closeTimestamp.ToString("T", new CultureInfo("es-ES"));
            string msg = $"Abierta: {openStr} | Cerrada: {closeStr} | Duracion: {duration.TotalSeconds:F1} segundos";
            duraciones.Add(msg);
            openTimestamp = null;
        }
    }

    public override string Nombre
    {
        get { return "Metrica Duracion Flor Abierta"; }
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
        duraciones = new List<string>();
        openTimestamp = null;
        ReiniciarPublicada();
    }

    protected override Modelo getModelo()
    {
        StringBuilder mensaje = new StringBuilder("");
        if (duraciones.Count > 0)
        {
            foreach (string evt in duraciones)
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