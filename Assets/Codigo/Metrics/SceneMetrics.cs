using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMetrics : MonoBehaviour
{
    private List<Metric> listaMetricasRegistradas = new List<Metric>();

    private void Start()
    {
        // Obt�n todas las instancias de Metric en la escena
        Metric[] metricas = GetComponentsInChildren<Metric>();

        // Suscribe el m�todo ManejarMetricaAlcanzada al evento de cada m�trica
        foreach (Metric metrica in metricas)
        {
            metrica.CondicionMetricaCumplida += RegistrarMetricaEscena;
        }
    }

    // Agrega la metrica que paso a cumplirse a la lista, mientras que la metrica sigue juntando informacion
    private void RegistrarMetricaEscena(Metric metrica)
    {
        listaMetricasRegistradas.Add(metrica);
        Debug.Log($"La metrica {metrica.Nombre} fue registrada en su escena.");
    }

    public void ReiniciarMetricasRegistradas()
    {
        Debug.Log("Se reinician las metricas de la escena");
        Metric[] metricas = GetComponentsInChildren<Metric>();

        // Reinicia cada metrica
        foreach (Metric metrica in metricas)
        {
            metrica.ReiniciarMetrica();
        }

        //Vacio la lista de registradas
        listaMetricasRegistradas.Clear();
    }

    public List<Metric> getMetricasRegistradas()
    {
        return new List<Metric>(listaMetricasRegistradas);
    }

}
