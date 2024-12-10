using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Globalization;

public class MetricaPosicionCabeza : Metric
{
    public float frecuenciaMuestreo = 10.0f;
    private List<string> mensajesDirecDetectadas = new List<string>();
    private Transform mainCamTransform;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("");
        mainCamTransform = GameObject.FindWithTag("MainCamera").transform;
        InvokeRepeating("check_Position_Angle", 1.0f, frecuenciaMuestreo);
        PublicarMetrica();
    }

    public void check_Position_Angle(){
        string currentTime = DateTime.Now.ToString("T", new CultureInfo("es-ES"));
        mensajesDirecDetectadas.Add($"{currentTime}: Posicion casco: {mainCamTransform.position.ToString()}");
        mensajesDirecDetectadas.Add($"{currentTime}: Rotacion casco: {mainCamTransform.eulerAngles.ToString()}");
        return;
    }

    public override string Nombre
    {
        get { return "Metrica posicion y angulo cabeza"; }
    }

    /// <summary>
    /// Funcion a sobrescribir por cada metrica, donde se llame a InvokeCondicionMetricaCumplida().
    /// </summary>
    /// <returns>void</returns>
    protected override void PublicarMetrica()
    {
        Debug.Log("Intento publicar la metrica");
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