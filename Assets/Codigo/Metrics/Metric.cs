using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class Metric : MonoBehaviour
{
    public abstract string Nombre { get; }
    public string NombreEscena;
    public delegate void MetricEventHandler(Metric metrica);
    public event MetricEventHandler CondicionMetricaCumplida;
    public bool metricaPublicada = false;

    protected abstract void PublicarMetrica();

    public abstract string toString();

    //Asi cada instancia puede invocar el evento para registrarse
    protected void InvokeCondicionMetricaCumplida()
    {
        CondicionMetricaCumplida?.Invoke(this);
        metricaPublicada = true;
    }

    void Awake()
    {
        NombreEscena = SceneManager.GetActiveScene().name;
        metricaPublicada = false;
    }
}
