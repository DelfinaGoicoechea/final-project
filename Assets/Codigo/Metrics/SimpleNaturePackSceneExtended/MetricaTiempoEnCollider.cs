using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Globalization;

public class MetricaTiempoEnCollider : Metric
{
    private List<string> duraciones = new List<string>();
    private DateTime? enterTimestamp = null;

    void Start()
    {
        PublicarMetrica();
    }

    // Llamar cuando el usuario entra en el collider
    public void OnEnterCollider()
    {
        if (enterTimestamp == null)
        {
            enterTimestamp = DateTime.Now;
        }
    }

    // Llamar cuando el usuario sale del collider
    public void OnExitCollider()
    {
        if (enterTimestamp != null)
        {
            DateTime exitTimestamp = DateTime.Now;
            TimeSpan duration = exitTimestamp - enterTimestamp.Value;
            string enterStr = enterTimestamp.Value.ToString("T", new CultureInfo("es-ES"));
            string exitStr = exitTimestamp.ToString("T", new CultureInfo("es-ES"));
            string msg = $"Entrada: {enterStr} | Salida: {exitStr} | Duracion: {duration.TotalSeconds:F1} segundos";
            duraciones.Add(msg);
            enterTimestamp = null;
        }
    }

    public override string Nombre
    {
        get { return "Metrica Tiempo en Collider Gorrion"; }
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
        enterTimestamp = null;
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