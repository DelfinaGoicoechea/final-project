using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Contents;

/// <summary>
/// Clase abstracta de la que deben heredar todas las metricas. 
/// Contiene los atributos y funciones indispensables para el funcionamiento del sistema de metricas.
/// </summary>
public abstract class Metric : MonoBehaviour
{
    public abstract string Nombre { get; }
    public string NombreContenido;
    public string Mensaje;
    public delegate void MetricEventHandler(Metric metrica);
    public event MetricEventHandler CondicionMetricaCumplida;
    public bool metricaPublicada = false;

    /// <summary>
    /// Funcion a sobrescribir por cada metrica, donde se llame a InvokeCondicionMetricaCumplida().
    /// </summary>
    /// <returns></returns>
    protected abstract void PublicarMetrica();
    
    /// <summary>
    /// Funcion a sobrescribir por cada metrica, donde se la reinicie.
    /// </summary>
    /// <returns></returns>
    public abstract void ReiniciarMetrica();
    
    /// <summary>
    /// Funcion a sobrescribir por cada metrica, donde se devuelva el modelo establecido debajo para enviarlo al cliente.
    /// </summary>
    /// <returns></returns>
    protected abstract Modelo getModelo();

    public class Modelo
    {
        public string Nombre;
        public string NombreContenido;
        public string Mensaje;
    }

    /// <summary>
    /// Funcion para que cada instancia invoque al evento de cumplimiento y se registre la metrica
    /// </summary>
    /// <returns></returns>
    protected void InvokeCondicionMetricaCumplida()
    {
        CondicionMetricaCumplida?.Invoke(this);
    }

    void Awake()
    {
        ReiniciarNombreContent();
        ReiniciarPublicada();
    }

    /// <summary>
    /// Funcion que setea el nombre del contenido actual a partir del Content Manager
    /// </summary>
    /// <returns></returns>
    protected void ReiniciarNombreContent()
    {
        ContentManager cManager = GameObject.FindWithTag("ContentManager").GetComponent<ContentManager>();
        NombreContenido = cManager.getPlayingName();
    }

    /// <summary>
    /// Funcion que indica que la metrica no se encuentra publicada.
    /// !! Usar por lo general en ReiniciarMetrica !!
    /// </summary>
    /// <returns></returns>
    protected void ReiniciarPublicada()
    {
        metricaPublicada = false;
    }

    /// <summary>
    /// Funcion que devuelve un json con la informacion de la metrica a partir de su modelo.
    /// </summary>
    /// <returns></returns>
    public string toString()
    {
        var modeloMetrica = getModelo();
        return JsonUtility.ToJson(modeloMetrica);
    }
}
