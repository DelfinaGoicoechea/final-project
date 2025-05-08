using UnityEngine;

public class WaveZoneTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el jugador ingresó a la zona de detección
        if (other.CompareTag("Player"))
        {
            // Busca el objeto del personaje en la escena
            GameObject character = GameObject.FindWithTag("Character");

            if (character != null)
            {
                // Obtiene el componente CharacterMovement del personaje
                CharacterMovement characterMovement = character.GetComponent<CharacterMovement>();

                if (characterMovement != null)
                {
                    // Llama al método de saludo del personaje
                    characterMovement.Wave();
                }
                else
                {
                    Debug.LogError("El componente CharacterMovement no está asignado al personaje.");
                }
            }
            else
            {
                Debug.LogError("No se encontró el personaje con la etiqueta 'Character'.");
            }
        }
    }
}
