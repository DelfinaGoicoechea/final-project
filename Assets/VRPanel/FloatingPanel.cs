using UnityEngine;

public class FloatingPanel : MonoBehaviour
{
    public Transform playerCamera;  // Cámara del jugador
    public float followDistance = 1.0f;  // Distancia del tablero
    public float heightOffset = 0.5f;  // Ajuste de altura del tablero

    void Update()
    {
        // Posiciona el panel frente a la cámara con un ajuste de altura
        Vector3 targetPosition = playerCamera.position + playerCamera.forward * followDistance - Vector3.up * heightOffset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 5);

        // Hace que el tablero mire siempre hacia la cámara
        transform.rotation = Quaternion.LookRotation(transform.position - playerCamera.position);
    }
}
