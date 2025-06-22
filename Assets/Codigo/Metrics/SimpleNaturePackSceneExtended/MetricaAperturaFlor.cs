using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Globalization;

public class MetricaAperturaFlor : Metric
{
    private List<string> eventosFlor = new List<string>();

    void Start()
    {
        PublicarMetrica();
    }
    
    // Llamado por GazeFlowerOpener cuando la flor se abre o cierra
    public void RegisterFlowerEvent(bool opened)
    {
        string estado = opened ? "abierta" : "cerrada";
        string currentTime = DateTime.Now.ToString("T", new CultureInfo("es-ES"));
        eventosFlor.Add($"{currentTime}: Flor {estado}");
    }

    public override string Nombre
    {
        get { return "Metrica Apertura Flor"; }
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
        eventosFlor = new List<string>();
        ReiniciarPublicada();
    }

    protected override Modelo getModelo()
    {
        StringBuilder mensaje = new StringBuilder("");
        if (eventosFlor.Count > 0)
        {
            foreach (string evt in eventosFlor)
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