using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MetricsManager : MonoBehaviour
{
    public static MetricsManager Instance { get; private set; }
    private GameObject sceneMetrics;
    private string nombreEscenaActual;


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

    public void BuildAndSendMetric()//args propios de la metrica
    {
        //Buildear Metrica en formato listo para ser enviado
        //Mandarselo al dispatcher
    }

    public void LevantarMetricas()//tipo de retorno del que requiera el dispatcher
    {
        //Por cada metric ademas que envie el nombre de la escena
    }

    // Start is called before the first frame update
    void Start()
    {
        sceneMetrics = GameObject.Find("SceneMetrics");
        SceneManager.activeSceneChanged += CambioEscena;
    }

    //Funcion que se activa cuando cambia la escena para poder tomar las metricas correctas
    void CambioEscena(Scene current, Scene next)
    {
        sceneMetrics = GameObject.Find("SceneMetrics");
        nombreEscenaActual = current.name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
