using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMetrics : MonoBehaviour
{
    private List<Metric> listaMetricasRegistradas = new List<Metric>();

    private void Start()
    {
        // Obtén todas las instancias de Metric en la escena
        Metric[] metricas = GetComponentsInChildren<Metric>();

        // Suscribe el método ManejarMetricaAlcanzada al evento de cada métrica
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

    public List<Metric> getMetricasRegistradas()
    {
        return listaMetricasRegistradas;
    }

}
