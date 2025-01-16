using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Globalization;

public class MetricaIzquierdaDerecha : Metric
{
    public float minAngle = 20.0f;
    private float lim_izq_frente;
    private float lim_der_frente;
    private float lim_der_atras;
    private List<string> mensajesDirecDetectadas = new List<string>();
    private Transform mainCamTransform;
    private string direcAnterior = "Discernible"; // Izquierda, Derecha, Centro, Detras

    // Start is called before the first frame update
    void Start()
    {
        // Asigno los valores de los limites de angulo
        lim_izq_frente = 180.0f - minAngle;
        lim_der_frente = 180.0f + minAngle;
        lim_der_atras = 360.0f - minAngle;
        // Agrego el MetricRayCaster a la camara principal
        mainCamTransform = GameObject.FindWithTag("MainCamera").transform;
        InvokeRepeating("checkAngle", 1.0f, 0.5f);
        PublicarMetrica();
    }

    void checkAngle(){
        // el angulo comienza en la nuca, y avanza en sentido horario.
        float y_angle = mainCamTransform.eulerAngles.y;

        // Derecha:
        //      [lim_der_frente, lim_der_atras) --> ej: con minAngle 20: [200, 340)
        if (y_angle >= lim_der_frente && y_angle < lim_der_atras && !direcAnterior.Equals("Derecha")){
            // Mira hacia la derecha
            direcAnterior = "Derecha";
            string currentTime = DateTime.Now.ToString("T", new CultureInfo("es-ES"));
            mensajesDirecDetectadas.Add($"{currentTime}: Mira hacia la derecha (angulo:{(int)y_angle})");
            return;
        }

        // Izquierda:
        //      [minAngle, lim_izq_frente) --> ej: con minAngle 20: [20, 160)
        if(y_angle >= minAngle && y_angle < lim_izq_frente && !direcAnterior.Equals("Izquierda")){
            // Mira hacia la izquierda
            direcAnterior = "Izquierda";
            string currentTime = DateTime.Now.ToString("T", new CultureInfo("es-ES"));
            mensajesDirecDetectadas.Add($"{currentTime}: Mira hacia la izquierda (angulo:{(int)y_angle})");
            return;
        }

        // Centro:
        //      [lim_izq_frente, lim_der_frente) --> ej: con minAngle 20: [160, 200)
        if(y_angle >= lim_izq_frente && y_angle < lim_der_frente && !direcAnterior.Equals("Centro")){
            // Mira hacia el centro
            direcAnterior = "Centro";
            string currentTime = DateTime.Now.ToString("T", new CultureInfo("es-ES"));
            mensajesDirecDetectadas.Add($"{currentTime}: Mira hacia el centro (angulo:{(int)y_angle})");
            return;
        }

        // Detras:
        //      [lim_der_atras, 360] o [0, minAngle) --> ej: con minAngle 20: [340, 360] o [0, 20)
        if((y_angle >= lim_der_atras || y_angle < minAngle) && !direcAnterior.Equals("Detras")){
            // Mira hacia atras
            direcAnterior = "Detras";
            string currentTime = DateTime.Now.ToString("T", new CultureInfo("es-ES"));
            mensajesDirecDetectadas.Add($"{currentTime}: Mira hacia detras (angulo:{(int)y_angle})");
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
