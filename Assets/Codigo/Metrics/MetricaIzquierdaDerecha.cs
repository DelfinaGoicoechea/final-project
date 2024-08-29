using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Globalization;

public class MetricaIzquierdaDerecha : Metric
{
    public float minAngle = 20.0f;
    private float maxAngle;
    private List<string> mensajesDirecDetectadas = new List<string>();
    private Transform mainCamTransform;
    private string direcAnterior = "Centro"; // Izquierda, Derecha, Centro

    // Start is called before the first frame update
    void Start()
    {
        // Agrego el MetricRayCaster a la camara principal
        maxAngle = 360.0f - minAngle;
        mainCamTransform = GameObject.FindWithTag("MainCamera").transform;
        InvokeRepeating("checkAngle",0, 0.5f);
        PublicarMetrica();
    }

    void checkAngle(){
        float y_angle = mainCamTransform.eulerAngles.y;
        if (y_angle >= minAngle && y_angle < 180.0f && !direcAnterior.Equals("Derecha")){
            // Mira hacia la derecha
            direcAnterior = "Derecha";
            string currentTime = DateTime.Now.ToString("T", new CultureInfo("es-ES"));
            this.mensajesDirecDetectadas.Add($"{currentTime}: Mira hacia la derecha ({y_angle}°)");
            return;
        }

        if(y_angle >= 180.0f && y_angle <= maxAngle && !direcAnterior.Equals("Izquierda")){
            // Mira hacia la izquierda
            direcAnterior = "Izquierda";
            string currentTime = DateTime.Now.ToString("T", new CultureInfo("es-ES"));
            this.mensajesDirecDetectadas.Add($"{currentTime}: Mira hacia la Izquierda ({y_angle}°)");
            return;
        }

        if((y_angle > maxAngle || y_angle < minAngle) && !direcAnterior.Equals("Centro")){
            // Mira hacia el centro
            direcAnterior = "Centro";
            string currentTime = DateTime.Now.ToString("T", new CultureInfo("es-ES"));
            this.mensajesDirecDetectadas.Add($"{currentTime}: Mira hacia el Centro ({y_angle}°)");
            return;
        }
    }

    public override string Nombre
    {
        get { return "MetricaIzquierdaDerecha"; }
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
        mensajesDirecDetectadas = new List<string>();
        ReiniciarPublicada();
    }
    
    /// <summary>
    /// Funcion a sobrescribir por cada metrica, donde se devuelva el modelo establecido debajo para enviarlo al cliente.
    /// </summary>
    /// <returns>Metric.Modelo</returns>
    protected override Modelo getModelo()
    {
        StringBuilder mensaje = new StringBuilder("");
        if(mensajesDirecDetectadas.Count > 0){
            foreach (string detectionMsg in mensajesDirecDetectadas){
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
}
