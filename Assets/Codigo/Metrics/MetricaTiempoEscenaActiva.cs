using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetricaTiempoEscenaActiva : Metric
{

    private int t;
    private int max;
    private const int threshold = 50;

    public override string Nombre
    {
        get { return "MetricaTiempoEscenaActiva"; }
    }

    private class Modelo
    {
        public string Nombre { get; set; }
        public string NombreEscena { get; set; }
        public int CantidadFramesEscena { get; set; }
        public string Mensaje { get; set; }
    }

    public override string toString()
    {
        var modeloMetrica = new Modelo
        {
            Nombre = Nombre,
            NombreEscena = NombreEscena,
            CantidadFramesEscena = max,
            Mensaje = "La escena estuvo demasiado tiempo activa."
        };

        return JsonUtility.ToJson(modeloMetrica);
    }

    protected bool CumpleConThreshold(int m)
    {
        if (m >= threshold)
        {
            return true;
        }
        return false;
    }

    protected override void PublicarMetrica()
    {
        if (CumpleConThreshold(max))
        {
            InvokeCondicionMetricaCumplida();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        t = 0;
        max = 0;
    }

    // Update is called once per frame
    void Update()
    {
        t++;
        if (t > max)
        {
            max = t;
        }
        if (max == threshold)
        {
            PublicarMetrica();
        }
    }
}
