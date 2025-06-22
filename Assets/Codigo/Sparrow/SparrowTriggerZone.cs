using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparrowTriggerZone : MonoBehaviour
{
    public SparrowIdleController sparrowController;
    public MetricaTiempoEnColliderGorrion metricaTiempo;

    void Start()
    {
        if(sparrowController == null)
        {
            Debug.LogError("SparrowController not assigned in Inspector on " + gameObject.name);
        }
        if (metricaTiempo == null)
        {
            Debug.LogError("MetricaTiempoEnColliderGorrion not assigned in Inspector on " + gameObject.name);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("MainCamera"))
            return;
        
        sparrowController.OnUserEnterZone();
        metricaTiempo?.OnEnterCollider();
    }

    private void OnTriggerExit(Collider other)
    {
        if(!other.CompareTag("MainCamera"))
            return;

        sparrowController.OnUserExitZone();
        metricaTiempo?.OnExitCollider();
    }
}
