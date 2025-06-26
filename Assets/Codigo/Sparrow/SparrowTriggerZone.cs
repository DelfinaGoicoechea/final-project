using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparrowTriggerZone : MonoBehaviour
{
    public SparrowIdleController sparrowController;

    void Start()
    {
        if(sparrowController == null)
        {
            Debug.LogError("SparrowController not assigned in Inspector on " + gameObject.name);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("MainCamera"))
            return;
        
        sparrowController.OnUserEnterZone();
    }

    private void OnTriggerExit(Collider other)
    {
        if(!other.CompareTag("MainCamera"))
            return;

        sparrowController.OnUserExitZone();
    }
}
