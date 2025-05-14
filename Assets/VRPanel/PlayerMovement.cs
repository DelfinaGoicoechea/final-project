using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 2.0f;
    
    // Variables para controlar el estado de movimiento
    private bool isMovingForward = false;
    private bool isMovingBackward = false;
    private bool isMovingLeft = false;
    private bool isMovingRight = false;

    void Update()
    {
        // Movimiento continuo en Update
        if (isMovingForward)
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
        if (isMovingBackward)
        {
            transform.Translate(-Vector3.forward * moveSpeed * Time.deltaTime);
        }
        if (isMovingLeft)
        {
            transform.Translate(-Vector3.right * moveSpeed * Time.deltaTime);
        }
        if (isMovingRight)
        {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        }
    }

    // Métodos públicos para activar/desactivar el movimiento
    public void StartMovingForward()
    {
        isMovingForward = true;
    }

    public void StopMovingForward()
    {
        isMovingForward = false;
    }

    public void StartMovingBackward()
    {
        isMovingBackward = true;
    }

    public void StopMovingBackward()
    {
        isMovingBackward = false;
    }

    public void StartMovingLeft()
    {
        isMovingLeft = true;
    }

    public void StopMovingLeft()
    {
        isMovingLeft = false;
    }

    public void StartMovingRight()
    {
        isMovingRight = true;
    }

    public void StopMovingRight()
    {
        isMovingRight = false;
    }
}