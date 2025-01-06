using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Clase que contiene el compertamiento de Metrics Manager, objeto que no se detruye en ejecucion y controla las metricas.
/// Es SINGLETON y no muere en toda la ejecucion.
/// </summary>
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

    /// <summary>
    /// Devuelve al Dispatcher una lista de todas las metricas a reportar en formato string(JSON)
    /// </summary>
    /// <returns></returns>
    public List<string> ReportarMetricas()
    {
        List<string> listaRetorno = listaMetricasRegistradas;
        listaMetricasRegistradas = new List<string>();
        return listaRetorno;
    }

    // Al iniciarse se subscribe al evento de cambio de escena
    void Start()
    {
        SceneManager.activeSceneChanged += CambioEscena;
    }

    /// <summary>
    /// Funcion que se activa cuando cambia la escena para poder tomar las metricas de la escena saliente
    /// </summary>
    /// <returns></returns>
    void CambioEscena(Scene current, Scene next)
    {
        LevantarMetricasEscena();
    }
    
    /// <summary>
    /// Agrega las metricas registradas por la escena a su lista de metricas ya regitradas en otras escenas (jsons)
    /// </summary>
    /// <returns></returns>
    public void LevantarMetricasEscena()
    {   
        // Para ello busca a Scene Metrics en dicha escena, le pide las metricas y reinicia su lista.
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

    /// <summary>
    /// Antes de terminar la sesion levanta las metricas de la ultima escena
    /// <summary>
    /// <returns></returns>
    void OnApplicationQuit()
    {
        LevantarMetricasEscena();
    }

}
