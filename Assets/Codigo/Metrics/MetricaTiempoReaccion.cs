using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetricaTiempoReaccion : MonoBehaviour {

    public DateTime tiempoCruzo;

    public DateTime? ultimoTiempoVerde = null;
    // Start is called before the first frame update
    public void cruzo(){
        tiempoCruzo = DateTime.Now;
        string tiempo = tiempoCruzo.ToString();
        TimeSpan diferencia;
        Debug.Log("Se registro el tiempo en el que se cruzo la calle:" + tiempo);
        if (ultimoTiempoVerde != null){
            diferencia = tiempoCruzo - ultimoTiempoVerde.Value;
            tiempo = $"{diferencia.Seconds}.{diferencia.Milliseconds:D3}";
            Debug.Log("El tiempo de reaccion fue de: " + tiempo);
        }else {Debug.Log("Se cruzo en rojo");}
        
        
    }

    public void tiempoVerde(){
        ultimoTiempoVerde = DateTime.Now;
        string tiempo = ultimoTiempoVerde.ToString();
        Debug.Log("Se registro el ultimo tiempo verde en: " + tiempo);
    }
}
