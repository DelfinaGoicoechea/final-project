using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MetricaTrayectoriaObjetosTomados : Metric
{
    public float frecuenciaDeCaptura = 1.0f;
    public List<string> mensajesTrayectorias = new();
    private Coroutine coroutineLadoIzq;
    private Coroutine coroutineLadoDer;
    private StringBuilder capturaIzq = new();
    private StringBuilder capturaDer = new();

    // Start is called before the first frame update
    void Start()
    {
        // Asigno sueltaFruta() al evento SelectExited en cada Interactor relevante del controlador
        ControllerReferences controllerReferences = GameObject.FindWithTag("ControllerReferences").GetComponent<ControllerReferences>();
        foreach (XRRayInteractor rayInteractor in controllerReferences.rayInteractors){
            rayInteractor.selectEntered.AddListener(tomaObjeto);
            rayInteractor.selectExited.AddListener(sueltaObjeto);
        }
        foreach (XRDirectInteractor directInteractor in controllerReferences.directInteractors){
            directInteractor.selectEntered.AddListener(tomaObjeto);
            directInteractor.selectExited.AddListener(sueltaObjeto);
        }
        PublicarMetrica();
    }

    public void tomaObjeto(SelectEnterEventArgs args) {
        bool isController = args.interactorObject.transform.parent.name.Contains("Controller");
        bool isLeftSide = args.interactorObject.transform.parent.name.Contains("Left");
        StringBuilder captura = isLeftSide ? capturaIzq : capturaDer;
        string interactuador;
        if (isController) {
            interactuador = isLeftSide ? "Joystick izquierdo" : "Joystick derecho";
        } else {
            interactuador = isLeftSide ? "Mano izquierda" : "Mano derecha";
        }
        captura.Append($"{interactuador} -> {args.interactableObject.transform.gameObject.name}\n");
        
        // Enviar a capturar trayectoria (startCapturing)
        if (isLeftSide){
            if (coroutineLadoIzq != null) {
                StopCoroutine(coroutineLadoIzq);
            }
            coroutineLadoIzq = StartCoroutine(startCapturing(
                args.interactableObject.transform,
                captura,
                frecuenciaDeCaptura
            ));
        } else {
            if (coroutineLadoDer != null){
                StopCoroutine(coroutineLadoDer);
            }
            coroutineLadoDer = StartCoroutine(startCapturing(
                args.interactableObject.transform,
                captura,
                frecuenciaDeCaptura
            ));
        }
    }

    private IEnumerator startCapturing(Transform frutaTransform, StringBuilder captura, float timeOut) {
        while (true) {
            string segundoSueltaStr = DateTime.Now.ToString("T", new CultureInfo("es-ES"));
            if (frutaTransform != null) {
                captura.Append($"{segundoSueltaStr}hs: {frutaTransform.position.ToString()},  ");
            }
            yield return new WaitForSeconds(timeOut);
        }
    }

    public void sueltaObjeto(SelectExitEventArgs args) {
        bool isLeftSide = args.interactorObject.transform.parent.name.Contains("Left");
        if (isLeftSide) {
            StopCoroutine(coroutineLadoIzq);
            coroutineLadoIzq = null;
            capturaIzq.Remove(capturaIzq.Length-3, 2); // quito la ultima coma y espacio
            mensajesTrayectorias.Add(capturaIzq.ToString());
            capturaIzq.Clear();
            
        } else {
            StopCoroutine(coroutineLadoDer);
            coroutineLadoDer = null;
            capturaDer.Remove(capturaDer.Length-3, 2); // quito la ultima coma y espacio
            mensajesTrayectorias.Add(capturaDer.ToString());
            capturaDer.Clear();
        }
        return;
    }

    public override string Nombre
    {
        get { return "Metrica Trayectoria Objetos Tomados"; }
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
        mensajesTrayectorias = new List<string>();
        ReiniciarPublicada();
    }
    
    /// <summary>
    /// Funcion a sobrescribir por cada metrica, donde se devuelva el modelo establecido debajo para enviarlo al cliente.
    /// </summary>
    /// <returns>Metric.Modelo</returns>
    protected override Modelo getModelo()
    {
        StringBuilder mensaje = new StringBuilder("");
        if(mensajesTrayectorias.Count > 0){
            foreach (string detectionMsg in mensajesTrayectorias){
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