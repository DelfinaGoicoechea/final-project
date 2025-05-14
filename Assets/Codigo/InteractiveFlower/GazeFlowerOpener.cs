using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeFlowerOpener : MonoBehaviour
{
    public Animator animator;

    // La flor solo se abrirá una vez
    private bool hasOpened = false;
    
    // Distancia máxima a la que el jugador puede mirar la flor para abrirla. 1 metro es la distancia predeterminada
    public float maxGazeDistance = 1.0f;
    
    void Start()
    {

        if (animator == null)
        {
            Debug.LogError("Animator not assigned in Inspector on " + gameObject.name);
        }
    }

    
    void Update()
    {
        if (hasOpened) return;

        // Guarda información sobre el objeto que el rayo golpea
        RaycastHit hit;

        // Crea un rayo desde la posición de la cámara y en la dirección hacia donde está mirando
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        // Si el rayo golpea un objeto y es la flor, la abre
        if (Physics.Raycast(ray, out hit, maxGazeDistance) && hit.collider.gameObject == gameObject)
        {
            animator.SetTrigger("LookAtTrigger");
            hasOpened = true;
        }
    }
}
