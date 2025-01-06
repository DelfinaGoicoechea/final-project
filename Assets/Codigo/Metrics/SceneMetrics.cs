using System.Collections.Generic;
using UnityEngine;

/// Clase que maneja las metricas dentro de una escena particular.
/// Toda escena que quiera medir sus metricas, debe contener Scene Metrics, porque se ella levanta las metricas el Metrics Manager.
public class SceneMetrics : MonoBehaviour
{
    // Mientras registra metricas, las conserva en una lista
    private List<Metric> listaMetricasRegistradas = new List<Metric>();

    private void Awake()
    {
        // Scene Metrics debe tener a todas las metricas que quiera utiliar en la escena, como hijos
        // Entonces crea una lista de metricas a recolectar a partir de sus hijos
        Metric[] metricas = GetComponentsInChildren<Metric>();

        // Se suscribe al evento CondicionMetricaCumplida de cada metrica, para que cuando una se cumpla, se la registre
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
