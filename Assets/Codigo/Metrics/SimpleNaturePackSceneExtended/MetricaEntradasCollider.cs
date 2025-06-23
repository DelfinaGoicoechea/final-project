using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Globalization;

public class MetricaEntradasCollider : Metric
{
    private int entradas = 0;
    private List<string> logs = new List<string>();

    void Start()
    {
        PublicarMetrica();
    }

    // Llamar cuando el usuario entra en el collider
    public void OnEnterCollider()
    {
        entradas++;
        string currentTime = DateTime.Now.ToString("T", new CultureInfo("es-ES"));
        logs.Add($"{currentTime}: Entrada al collider del gorrion. Total: {entradas}");
    }

    public override string Nombre
    {
        get { return "Metrica Entradas Collider Gorrion"; }
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
        entradas = 0;
        logs = new List<string>();
        ReiniciarPublicada();
    }

    protected override Modelo getModelo()
    {
        StringBuilder mensaje = new StringBuilder("");
        if (logs.Count > 0)
        {
            foreach (string evt in logs)
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