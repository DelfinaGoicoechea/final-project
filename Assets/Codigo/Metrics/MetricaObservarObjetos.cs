using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class MetricaObservarObjetos : Metric
{
    private List<string> mensajesObjDetectados = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        // Agrego el MetricRayCaster a la camara principal
        GameObject mainCamera = GameObject.FindWithTag("MainCamera").gameObject;
        MetricRayCaster mrc = mainCamera.AddComponent<MetricRayCaster>();
        mrc.metric = this;
        PublicarMetrica();
    }

    public override string Nombre
    {
        get { return "Metrica Observar Objetos"; }
    }

    /// <summary>
    /// Funcion a sobrescribir por cada metrica, donde se llame a InvokeCondicionMetricaCumplida().
    /// </summary>
    /// <returns>void</returns>
    protected override void PublicarMetrica()
    {
         // ver condicion (decidir si publicar siempre, o solo cuando se detecta un objeto)
        if (!metricaPublicada) 
        {
            InvokeCondicionMetricaCumplida();
            metricaPublicada = true;
        }
    }

    /// <summary>
    /// Funcion a sobrescribir por cada metrica, donde se la reinicie.
    /// </summary>
    /// <returns>void</returns>
    public override void ReiniciarMetrica()
    {
        mensajesObjDetectados = new List<string>();
        ReiniciarPublicada();
    }
    
    /// <summary>
    /// Funcion a sobrescribir por cada metrica, donde se devuelva el modelo establecido debajo para enviarlo al cliente.
    /// </summary>
    /// <returns>Metric.Modelo</returns>
    protected override Modelo getModelo()
    {
        StringBuilder mensaje = new StringBuilder("");
        if(mensajesObjDetectados.Count > 0){
            foreach (string detectionMsg in mensajesObjDetectados){
                mensaje.Append($"{detectionMsg}\n");
            }
            // quito el ultimo \n
            mensaje = mensaje.Remove(mensaje.Length-1, 1);
        }

        var modeloMetrica = new Modelo
        {
            Nombre = Nombre,
            NombreContenido = NombreContenido,
            Mensaje = mensaje.ToString(),
        };
        return modeloMetrica;
    }

    /// <summary>
    /// Metodo para agregar un mensaje al detectar un objeto por parte del Raycaster 
    /// </summary>
    /// <returns>Metric.Modelo</returns>
    public void addObjectMessage(string message){
        this.mensajesObjDetectados.Add(message);
    }
}
