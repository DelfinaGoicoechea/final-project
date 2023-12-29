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

    private class Modelo
    {
        public string Nombre;
        public string NombreContenido;
        public float TiempoMaxTranscurrido;
        public string Mensaje;
    }

    public override string toString()
    {
        var modeloMetrica = new Modelo
        {
            Nombre = Nombre,
            NombreContenido = NombreContenido,
            TiempoMaxTranscurrido = tiempoMaxTranscurrido,
            Mensaje = "La escena estuvo demasiado tiempo activa."
        };

        return JsonUtility.ToJson(modeloMetrica);
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
        tiempoTranscurrido = 0;
        tiempoMaxTranscurrido = 0;
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
            PublicarMetrica();
        }
    }
}
