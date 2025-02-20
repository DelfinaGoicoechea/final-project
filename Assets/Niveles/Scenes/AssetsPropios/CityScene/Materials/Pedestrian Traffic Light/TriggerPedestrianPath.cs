using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPedestrianPath : MonoBehaviour {

    public MetricaTiempoReaccion metricaTiempoReaccion;
    // Start is called before the first frame update
   

    private void OnTriggerEnter(Collider other){
        metricaTiempoReaccion.cruzo();
    }
    

}
