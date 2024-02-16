using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetricaTiempoEscenaActiva : Metric
{

    public float tiempoLimite = 5f;
    private float tiempoTranscurrido = 0f;
    private float tiempoMaxTranscurrido = 0f;

    public override string Nombre
    {
        get { return "MetricaTiempoEscenaActiva"; }
    }

    public override Modelo getModelo()
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
