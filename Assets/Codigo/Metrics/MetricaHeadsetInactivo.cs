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
        public string Nombre { get; set; }
        public string NombreEscena { get; set; }
        public float CantidadSegundosInactivo { get; set; }
        public string Mensaje { get; set; }
    }

    public override string toString()
    {
        var modeloMetrica = new Modelo
        {
            Nombre = Nombre,
            NombreEscena = NombreEscena,
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
                PublicarMetrica();
            }
        }

        lastCameraTransform.position = camera.position;
        lastCameraTransform.rotation = camera.rotation;
    }
}

