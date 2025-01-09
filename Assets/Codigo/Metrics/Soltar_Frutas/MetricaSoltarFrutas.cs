using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MetricaSoltarFrutas : Metric
{
    public List<string> mensajesFrutas = new();
    private Dictionary<string, bool> enCajas = new(); // key: transform de fruta, value: si esta en caja correcta
    private Dictionary<string, DateTime> soltados = new();

    // Start is called before the first frame update
    void Start()
    {
        // Asigno sueltaFruta() al evento SelectExited en cada Interactor relevante del controlador
        ControllerReferences controllerReferences = GameObject.FindWithTag("ControllerReferences").GetComponent<ControllerReferences>();
        foreach (XRRayInteractor rayInteractor in controllerReferences.rayInteractors){
            rayInteractor.selectExited.AddListener(sueltaFruta);
        }
        foreach (XRDirectInteractor directInteractor in controllerReferences.directInteractors){
            directInteractor.selectExited.AddListener(sueltaFruta);
        }
        PublicarMetrica();
    }

    public void EntraEnCajon(string frutaObjectName, bool isCajonCorrecto) {
        if (!soltados.ContainsKey(frutaObjectName)) {
            enCajas[frutaObjectName] = isCajonCorrecto;
            return;
        } else {
            string segundoSueltaStr = soltados[frutaObjectName].ToString("T", new CultureInfo("es-ES"));
            soltados.Remove(frutaObjectName);
            string isCajonCorrectoStr = isCajonCorrecto ? "correcto" : "incorrecto";
            mensajesFrutas.Add($"{segundoSueltaStr}: Se solto fruta en cajon {isCajonCorrectoStr}");
        }
    }

    public void sueltaFruta(SelectExitEventArgs args) {
        string frutaObjectName = args.interactableObject.transform.gameObject.name;
        DateTime currentTime = DateTime.Now;
        if (enCajas.ContainsKey(frutaObjectName)) {
            string isCajonCorrecto = enCajas[frutaObjectName] ? "correcto" : "incorrecto";
            enCajas.Remove(frutaObjectName);
            string currTimeStr = DateTime.Now.ToString("T", new CultureInfo("es-ES"));
            mensajesFrutas.Add($"{currTimeStr}: Se solto fruta en cajon {isCajonCorrecto}");
        } else {
            soltados[frutaObjectName] = currentTime;
            recheckConTimeout(frutaObjectName, currentTime,  5);
        }
        return;
    }

    private IEnumerable recheckConTimeout(string frutaObjectName, DateTime segSuelta, float timeOut) {
        yield return new WaitForSeconds(timeOut);
        if (soltados.ContainsKey(frutaObjectName)) {
            string segundoSueltaStr = segSuelta.ToString("T", new CultureInfo("es-ES"));
            mensajesFrutas.Add($"{segundoSueltaStr}: Se solto fruta fuera de los cajones");
            soltados.Remove(frutaObjectName);
        }
    }

    public void cambiaDificultad(string nuevoNivel) {
        string currTimeStr = DateTime.Now.ToString("T", new CultureInfo("es-ES"));
        Debug.Log($"{currTimeStr}: Se encuentra en el estado \"{nuevoNivel}\"");
        mensajesFrutas.Add($"{currTimeStr}: Se encuentra en el estado \"{nuevoNivel}\"");
    }
    public override string Nombre
    {
        get { return "Metrica soltar frutas"; }
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
        mensajesFrutas = new List<string>();
        ReiniciarPublicada();
    }
    
    /// <summary>
    /// Funcion a sobrescribir por cada metrica, donde se devuelva el modelo establecido debajo para enviarlo al cliente.
    /// </summary>
    /// <returns>Metric.Modelo</returns>
    protected override Modelo getModelo()
    {
        StringBuilder mensaje = new StringBuilder("");
        if(mensajesFrutas.Count > 0){
            foreach (string detectionMsg in mensajesFrutas){
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