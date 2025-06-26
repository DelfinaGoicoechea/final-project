using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparrowTriggerZone : MonoBehaviour
{
    public SparrowIdleController sparrowController;
    public MetricaTiempoEnCollider metricaTiempo;
    public MetricaEntradasCollider metricaEntradasCollider;

    void Start()
    {
        if(sparrowController == null)
        {
            Debug.LogError("SparrowController not assigned in Inspector on " + gameObject.name);
        }
        if (metricaTiempo == null)
        {
            Debug.LogError("MetricaTiempo not assigned in Inspector on " + gameObject.name);
        }
        if (metricaEntradasCollider == null)
        {
            Debug.LogError("MetricaEntradasCollider not assigned in Inspector on " + gameObject.name);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("MainCamera"))
            return;
        
        sparrowController.OnUserEnterZone();
        metricaTiempo?.OnEnterCollider();
        metricaEntradasCollider?.OnEnterCollider();
    }

    private void OnTriggerExit(Collider other)
    {
        if(!other.CompareTag("MainCamera"))
            return;

        sparrowController.OnUserExitZone();
        metricaTiempo?.OnExitCollider();
    }
}
