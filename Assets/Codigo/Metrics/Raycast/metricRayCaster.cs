using UnityEngine;
using System;
using System.Globalization;

public class MetricRayCaster : MonoBehaviour
{
    private Camera playerCamera;
    private Transform lastHitTransform = null;
    public MetricaObservarObjetos metric;
    
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
                if(hit.transform.TryGetComponent<MetricRayReceiver>(out MetricRayReceiver rayReceiver)){
                    if(hit.transform != lastHitTransform){ // emite una sola vez aunque lo siga mirando.
                        lastHitTransform = hit.transform;
                        string msg = rayReceiver.getMessage();
                        string currentTime = DateTime.Now.ToString("T", new CultureInfo("es-ES"));
                        this.metric.addObjectMessage($"{currentTime}: {rayReceiver.getMessage()}");
                    }
                }
        } else { // reinicio lastHitTransform cuando deja de haber un hit (ya no lo mira)
                lastHitTransform = null;
        } 
    }
}
