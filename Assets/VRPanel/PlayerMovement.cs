using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 2.0f;

    public void MoveForward()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    public void MoveBackward()
    {
        transform.Translate(-Vector3.forward * moveSpeed * Time.deltaTime);
    }

    public void MoveLeft()
    {
        transform.Translate(-Vector3.right * moveSpeed * Time.deltaTime);
    }

    public void MoveRight()
    {
        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
    }
}
