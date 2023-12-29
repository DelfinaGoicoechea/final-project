using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetricaHeadsetInactivo : Metric
{

    public float translationThreshold = 0.001f;
    public float rotationThreshold = 0.001f;
    public float tiempoLimite = 5f;
    public float segundosInactivo;
    public float segundosMaxInactivo;
    private Transform lastCameraTransform;

    public override string Nombre
    {
        get { return "MetricaHeadsetInactivo"; }
    }

    private class Modelo
    {
        public string Nombre;
        public string NombreContenido;
        public float CantidadSegundosInactivo;
        public string Mensaje;
    }

    public override string toString()
    {
        var modeloMetrica = new Modelo
        {
            Nombre = Nombre,
            NombreContenido = NombreContenido,
            CantidadSegundosInactivo = segundosMaxInactivo,
            Mensaje = "Headset inactivo por mucho tiempo."
        };

        return JsonUtility.ToJson(modeloMetrica);
    }

    protected override void PublicarMetrica()
    {
        InvokeCondicionMetricaCumplida();
    }

    void Start()
    {
        lastCameraTransform = Camera.main.transform;
        segundosInactivo = 0f;
        segundosMaxInactivo = 0f;
    }

    public override void ReiniciarMetrica()
    {
        lastCameraTransform = Camera.main.transform;
        segundosInactivo = 0f;
        segundosMaxInactivo = 0f;
        metricaPublicada = false;
    }

    void Update()
    {
        var camera = Camera.main.transform;

        bool moved = (camera.position - lastCameraTransform.position).sqrMagnitude > translationThreshold;
        bool turned = Quaternion.Dot(camera.rotation, lastCameraTransform.rotation) < (1f - rotationThreshold);

        if (moved || turned)
        {
            segundosInactivo = 0f;
        }
        else
        {
            segundosInactivo += Time.deltaTime;
            if (segundosInactivo > segundosMaxInactivo)
            {
                segundosMaxInactivo = segundosInactivo;
            }
            if ((!metricaPublicada)&&(segundosMaxInactivo >= tiempoLimite))
            {
                metricaPublicada = true;
                PublicarMetrica();
            }
        }

        lastCameraTransform.position = camera.position;
        lastCameraTransform.rotation = camera.rotation;
    }
}

