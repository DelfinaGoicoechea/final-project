using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Metrica que mide el tiempo que una escena paso activa.
/// A fines practicos no resulta demasiado util, pero demuestra el funcionamiento de las metricas.
/// </summary>
public class MetricaTiempoEscenaActiva : Metric
{

    // Primero define las variables de funcionamiento y threshold para la completitud
    public float tiempoLimite = 5f;
    private float tiempoTranscurrido = 0f;
    private float tiempoMaxTranscurrido = 0f;

    // Nombre representativo de la metrica medida
    public override string Nombre
    {
        get { return "MetricaTiempoEscenaActiva"; }
    }

    // El nombre de la metrica y de la escena ya estan dados
    // Se debe ingresar, a partir de los datos recolectados, el mensaje que se mostrara al cliente
    // Posible mejora: ingresar datos crudos y serializar de otra forma en cliente
    protected override Modelo getModelo()
    {
        string m = "";
        if (tiempoMaxTranscurrido >= 60)
        {
            int minutes = (int)Math.Round(tiempoMaxTranscurrido / 60);
            m = $"La escena estuvo demasiado tiempo activa: {minutes} minutos";
            }
        else
        {
            int segundos = (int)Math.Round(tiempoMaxTranscurrido);
            m = $"La escena estuvo demasiado tiempo activa: {segundos} segundos";
        };

        var modeloMetrica = new Modelo
        {
            Nombre = Nombre,
            NombreContenido = NombreContenido,
            Mensaje = m
        };

        return modeloMetrica;
    }

    public override void ReiniciarMetrica()
    {
        tiempoTranscurrido = 0f;
        tiempoMaxTranscurrido = 0f;
        ReiniciarPublicada();
    }

    protected bool CumpleConThreshold(float m)
    {
        if (m >= tiempoLimite)
        {
            return true;
        }
        return false;
    }

    protected override void PublicarMetrica()
    {
        if (CumpleConThreshold(tiempoMaxTranscurrido))
        {
            InvokeCondicionMetricaCumplida();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        tiempoTranscurrido = 0f;
        tiempoMaxTranscurrido = 0f;
    }

    // Update is called once per frame
    // Por lo general la informacion que levantan las metricas, si tiene que ver con tiempo de actividad, 
    // debe ser actualizada en Update.
    void Update()
    {
        tiempoTranscurrido += Time.deltaTime;
        if (tiempoTranscurrido > tiempoMaxTranscurrido)
        {
            tiempoMaxTranscurrido = tiempoTranscurrido;
        }
        if ((!metricaPublicada)&&(tiempoMaxTranscurrido >= tiempoLimite))
        {
            ReiniciarNombreContent();
            metricaPublicada = true;
            PublicarMetrica();
        }
    }
}
