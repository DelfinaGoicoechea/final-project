using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetricaTiempoReaccion : Metric {

    private DateTime tiempoCruzo;

    private DateTime? ultimoTiempoVerde = null;

    private string tiempoReaccion;
    // Start is called before the first frame update
    public void cruzo(){
        tiempoCruzo = DateTime.Now;
        string tiempo = tiempoCruzo.ToString();
        TimeSpan diferencia;
        Debug.Log("Se registro el tiempo en el que se cruzo la calle:" + tiempo);
        if (ultimoTiempoVerde != null){
            diferencia = tiempoCruzo - ultimoTiempoVerde.Value;
            tiempoReaccion = $"{diferencia.Seconds}.{diferencia.Milliseconds:D3}";
            Debug.Log("El tiempo de reaccion fue de: " + tiempoReaccion);
        }else {Debug.Log("Se cruzo en rojo");}
        
        
    }

    public void tiempoVerde(){
        ultimoTiempoVerde = DateTime.Now;
        string tiempo = ultimoTiempoVerde.ToString();
        Debug.Log("Se registro el ultimo tiempo verde en: " + tiempo);
    }

    public override string Nombre
    {
        get { return "Metrica tiempo de reaccion"; }
    }

    protected override void PublicarMetrica()
    {
        // ver condicion (decidir si publicar siempre, o solo cuando se detecta un objeto)
        if (!metricaPublicada) 
        {
            InvokeCondicionMetricaCumplida();
            metricaPublicada = true;
        }
    }

    public override void ReiniciarMetrica()
    {
        tiempoReaccion = null;
        ReiniciarPublicada();
    }

    protected override Modelo getModelo()
    {
        throw new NotImplementedException();
    }
}
