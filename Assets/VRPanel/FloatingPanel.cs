using UnityEngine;

public class FloatingPanel : MonoBehaviour
{
    public Transform playerCamera;  // Cámara del jugador
    public float followDistance = 1.0f;  // Distancia del tablero
    public float heightOffset = 0.5f;  // Ajuste de altura
    public float rotationSmoothness = 5.0f;  // Suavizado de rotación

    void Update()
    {
        // Posición: Fija frente a la cámara (sin retraso)
        Vector3 targetPosition = playerCamera.position + 
                                playerCamera.forward * followDistance - 
                                Vector3.up * heightOffset;
        
        transform.position = targetPosition;  // Sin Lerp para movimiento

        // Rotación: Suavizada para evitar saltos bruscos
        Quaternion targetRotation = Quaternion.LookRotation(transform.position - playerCamera.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSmoothness);
    }
}