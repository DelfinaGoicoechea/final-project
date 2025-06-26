using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 2.0f;
    public Transform headset; // Arrastra la cámara VR o la cabeza aquí

    // Variables de control
    private bool isMovingForward = false;
    private bool isMovingBackward = false;
    private bool isMovingLeft = false;
    private bool isMovingRight = false;

    void Update()
    {
        Vector3 moveDirection = Vector3.zero;

        // Calcula la dirección relativa a la cabeza
        if (isMovingForward)
            moveDirection += headset.forward;
        if (isMovingBackward)
            moveDirection -= headset.forward;
        if (isMovingLeft)
            moveDirection -= headset.right;
        if (isMovingRight)
            moveDirection += headset.right;

        // Elimina el componente vertical (opcional) y normaliza
        moveDirection.y = 0;
        if (moveDirection.magnitude > 0)
        {
            moveDirection.Normalize();
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    // Funciones públicas para los botones (¡asegúrate de que sean PUBLIC!)
    public void StartMovingForward() => isMovingForward = true;
    public void StopMovingForward() => isMovingForward = false;

    public void StartMovingBackward() => isMovingBackward = true;
    public void StopMovingBackward() => isMovingBackward = false;

    public void StartMovingLeft() => isMovingLeft = true;
    public void StopMovingLeft() => isMovingLeft = false;

    public void StartMovingRight() => isMovingRight = true;
    public void StopMovingRight() => isMovingRight = false;
}