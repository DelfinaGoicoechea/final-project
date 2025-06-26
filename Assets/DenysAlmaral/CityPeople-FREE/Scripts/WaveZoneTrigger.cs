using UnityEngine;

public class WaveZoneTrigger : MonoBehaviour
{
    [SerializeField]
    CharacterMovement characterMovement; // Referencia al componente CharacterMovement asignada en Editor

    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el jugador ingresó a la zona de detección
        if (other.CompareTag("Player"))
        {
            if (characterMovement != null)
            {
                // Llama al método de saludo del personaje
                characterMovement.Wave();
                Debug.Log("Se entro en la zona");
            }
            else
            {
                Debug.LogError("El componente CharacterMovement no está asignado al personaje.");
            }
        }
    }
}
