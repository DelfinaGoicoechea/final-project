using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MetricsManager : MonoBehaviour
{
    public static MetricsManager Instance { get; private set; }
    private SceneMetrics sceneMetrics;
    private List<string> listaMetricasRegistradas = new List<string>();


    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // Devuelve al Dispatcher una lista de las metricas en formato string(JSON)
    public List<string> ReportarMetricas()
    {
        List<string> listaRetorno = listaMetricasRegistradas;
        listaMetricasRegistradas = new List<string>();
        Debug.Log(listaRetorno);
        return listaRetorno;
    }

    // Al iniciarse se subscribe al evento de cambio de escena
    void Start()
    {
        SceneManager.activeSceneChanged += CambioEscena;
    }

    // Funcion que se activa cuando cambia la escena para poder tomar las metricas de la escena saliente
    void CambioEscena(Scene current, Scene next)
    {
        LevantarMetricasEscena();
    }
    
    //Agrega las metricas registradas por la escena a su lista de jsons
    public void LevantarMetricasEscena()
    {
        GameObject metricsGameObject = GameObject.Find("SceneMetrics");
        if (metricsGameObject != null)
        {
            sceneMetrics = metricsGameObject.GetComponent<SceneMetrics>();
            List<Metric> metricasEscena = sceneMetrics.getMetricasRegistradas();
            foreach (Metric metrica in metricasEscena)
            {
                string metricaBuildeada = metrica.toString();
                listaMetricasRegistradas.Add(metricaBuildeada);
            }
            sceneMetrics.ReiniciarMetricasRegistradas();
        }
    }

    // Levanta las metricas de la ultima escena
    void OnApplicationQuit()
    {
        LevantarMetricasEscena();
    }

}
