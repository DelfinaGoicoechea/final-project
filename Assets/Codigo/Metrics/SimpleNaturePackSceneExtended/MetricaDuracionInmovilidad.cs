using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Globalization;

public class MetricaDuracionInmovilidad : Metric
{
    private List<string> duraciones = new List<string>();
    private DateTime? stillnessStartTime = null;

    void Start()
    {
        PublicarMetrica();
    }

    // Llamar cuando el usuario comienza a estar inmovil (musica en volumen minimo)
    public void OnStillnessStart()
    {
        if (stillnessStartTime == null)
        {
            stillnessStartTime = DateTime.Now;
        }
    }

    // Llamar cuando el usuario comienza a moverse nuevamente
    public void OnStillnessEnd()
    {
        if (stillnessStartTime != null)
        {
            DateTime stillnessEndTime = DateTime.Now;
            TimeSpan duration = stillnessEndTime - stillnessStartTime.Value;
            string startStr = stillnessStartTime.Value.ToString("T", new CultureInfo("es-ES"));
            string endStr = stillnessEndTime.ToString("T", new CultureInfo("es-ES"));
            string msg = $"Inmovilidad: {startStr} | Movimiento: {endStr} | Duracion: {duration.TotalSeconds:F1} segundos";
            duraciones.Add(msg);
            stillnessStartTime = null;
        }
    }

    public override string Nombre
    {
        get { return "Metrica Duracion Inmovilidad"; }
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
        stillnessStartTime = null;
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