using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.PhysicsModule;

public class metricRayCaster : MonoBehaviour
{
    private Camera playerCamera;
    private Transform lastHitTransform = null;
    
    // Start is called before the first frame update
    void Start()
    {
        playerCamera = gameObject.GetComponent<Camera>();
        InvokeRepeating("LaunchRay", 1.0f, 0.3f);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void LaunchRay()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if(Physics.Raycast(ray, out RaycastHit hit, 100f)){
                if(hit.transform.TryGetComponent<MetricRayReceiver>(out MetricRayReceiver mrr)){
                    if(hit.transform != lastHitTransform){ // emite una sola vez aunque lo siga mirando.
                        lastHitTransform = hit.transform;
                        Debug.Log(mrr.getMessage());
                    }
                }
        } else { // reinicio lastHitTransform cuando deja de haber un hit (ya no lo mira)
                lastHitTransform = null;
        } 
    }
}
