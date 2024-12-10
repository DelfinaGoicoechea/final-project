using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class MetricaPosicionManosJoystick : Metric
{
    public float frecuenciaMuestreo = 10.0f;
    private List<string> mensajesDirecDetectadas = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        // Busco los GameObjects que me interesan
        Transform camOffsetTransform = GameObject.FindWithTag("CameraOffset").transform;
        GameObject leftJoystick = camOffsetTransform.Find("Left Controller").gameObject;
        GameObject rightJoystick = camOffsetTransform.Find("Right Controller").gameObject;
        GameObject leftHand = camOffsetTransform.Find("Left Hand").gameObject;
        GameObject rightHand = camOffsetTransform.Find("Right Hand").gameObject;
        // Joystick Izquierdo
        CapturadorPosicion capturador_JoyI = leftJoystick.AddComponent<CapturadorPosicion>();
        capturador_JoyI.frecuenciaMuestreo = 10.0f;
        capturador_JoyI.nombreObjeto = "Joystick Izq";
        capturador_JoyI.metricaPrincipal = this;
        // Joystick Derecho
        CapturadorPosicion capturador_JoyD = rightJoystick.AddComponent<CapturadorPosicion>();
        capturador_JoyD.frecuenciaMuestreo = 10.0f;
        capturador_JoyD.nombreObjeto = "Joystick Der";
        capturador_JoyD.metricaPrincipal = this;
        // Mano Izquierda
        CapturadorPosicion capturador_ManoI = leftHand.AddComponent<CapturadorPosicion>();
        capturador_ManoI.frecuenciaMuestreo = 10.0f;
        capturador_ManoI.nombreObjeto = "Mano Izq";
        capturador_ManoI.metricaPrincipal = this;
        // Mano Derecha
        CapturadorPosicion capturador_ManoD = rightHand.AddComponent<CapturadorPosicion>();
        capturador_ManoD.frecuenciaMuestreo = 10.0f;
        capturador_ManoD.nombreObjeto = "Mano Der";
        capturador_ManoD.metricaPrincipal = this;
        // Comenzar captura
        capturador_JoyI.startPolling();
        capturador_ManoI.startPolling();
        capturador_JoyD.startPolling();
        capturador_ManoD.startPolling();
        PublicarMetrica();
    }

    public override string Nombre
    {
        get { return "Metrica posicion y angulo Manos y Joysticks"; }
    }

    public void addMessage(string msg) {
        mensajesDirecDetectadas.Add(msg);
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