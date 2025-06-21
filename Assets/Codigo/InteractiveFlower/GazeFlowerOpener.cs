using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeFlowerOpener : MonoBehaviour
{
    public Animator animator;
    public MetricaAperturaFlor metricaAperturaFlor;
    public MetricaDuracionFlorAbierta metricaDuracionFlorAbierta;

    // The flower will close if the user is not looking at it
    private bool isLooking = false;
    
    // Distancia máxima a la que el jugador puede mirar la flor para abrirla
    public float maxGazeDistance = 6.0f;
    
    void Start()
    {

        if (animator == null)
        {
            Debug.LogError("Animator not assigned in Inspector on " + gameObject.name);
        }
        if (metricaAperturaFlor == null)
        {
            Debug.LogError("MetricaAperturaFlor not assigned in Inspector on " + gameObject.name);
        }
        if (metricaDuracionFlorAbierta == null)
        {
            Debug.LogError("MetricaDuracionFlorAbierta not assigned in Inspector on " + gameObject.name);
        }
    }

    
    void Update()
    {
        // Guarda información sobre el objeto que el rayo golpea
        RaycastHit hit;

        // Crea un rayo desde la posición de la cámara y en la dirección hacia donde está mirando
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        // Check if the raycast hits the flower
        bool currentlyLooking = Physics.Raycast(ray, out hit, maxGazeDistance) && hit.collider.gameObject == gameObject;

        // If the raycast hits the flower, open it
        if (currentlyLooking != isLooking)
        {
            isLooking = currentlyLooking;
            animator.SetBool("IsLooking", isLooking);
            metricaAperturaFlor.RegisterFlowerEvent(isLooking);
            if (isLooking)
            {
                metricaDuracionFlorAbierta.OnFlowerOpened();
            }
            else
            {
                metricaDuracionFlorAbierta.OnFlowerClosed();
            }
        }
    }
}
