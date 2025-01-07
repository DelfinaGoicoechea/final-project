using System;
using UnityEngine;
using System.Globalization;

public class CapturadorPosicion : MonoBehaviour
{
    public float frecuenciaMuestreo = 10.0f;
    public string nombreObjeto = "FaltaNombre";
    public MetricaPosicionManosJoystick metricaPrincipal;
    private bool alreadyInitialized = false;

    public void startPolling() {
        if (gameObject.activeSelf) { 
            InvokeRepeating("check_Position_Angle", 3.0f, frecuenciaMuestreo);
        }
        alreadyInitialized = true; // Util para asegurarse que ya se seteo frecuencuaMuestreo al llegar a OnEnable()
    }

    void OnEnable() {
        if (alreadyInitialized) {
            InvokeRepeating("check_Position_Angle", 3.0f, frecuenciaMuestreo);
        }
    }

    void OnDisable() {
        CancelInvoke();
    }

    public void check_Position_Angle() {
        string currentTime = DateTime.Now.ToString("T", new CultureInfo("es-ES"));
        metricaPrincipal.addMessage($"{currentTime}: Posicion {nombreObjeto}: {gameObject.transform.position.ToString()}");
        metricaPrincipal.addMessage($"{currentTime}: Rotacion {nombreObjeto}: {gameObject.transform.eulerAngles.ToString()}");
        return;
    }
}