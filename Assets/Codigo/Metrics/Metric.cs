using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Contents;

public abstract class Metric : MonoBehaviour
{
    public abstract string Nombre { get; }
    public string NombreContenido;
    public string Mensaje;
    public delegate void MetricEventHandler(Metric metrica);
    public event MetricEventHandler CondicionMetricaCumplida;
    public bool metricaPublicada = false;

    protected abstract void PublicarMetrica();
    public abstract void ReiniciarMetrica();
    public abstract Modelo getModelo();

    public class Modelo
    {
        public string Nombre;
        public string NombreContenido;
        public string Mensaje;
    }

    //Asi cada instancia puede invocar el evento para registrarse
    protected void InvokeCondicionMetricaCumplida()
    {
        CondicionMetricaCumplida?.Invoke(this);
    }

    void Awake()
    {
        ReiniciarNombreContent();
        ReiniciarPublicada();
    }

    protected void ReiniciarNombreContent()
    {
        ContentManager cManager = GameObject.FindWithTag("ContentManager").GetComponent<ContentManager>();
        NombreContenido = cManager.getPlayingName();
    }

    protected void ReiniciarPublicada()
    {
        metricaPublicada = false;
    }

    public string toString()
    {
        var modeloMetrica = getModelo();
        return JsonUtility.ToJson(modeloMetrica);
    }
}
