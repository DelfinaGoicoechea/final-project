using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private Animator animator;
    private bool isWalking = false;
    private bool hasWaved = false;

    public float speed = 2f;

    void Start()
    {
        animator = GetComponent<Animator>();

        // Inicialmente el personaje está parado
        animator.SetBool("isWalking", false);
    }

    void Update()
    {
        if (isWalking)
        {
            // Mover hacia adelante mientras camina
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    public void StartWalking()
    {
        isWalking = true;
        animator.SetBool("isWalking", true);
    }

    public void Wave()
    {
        if (!hasWaved)
        {
            animator.SetTrigger("wave");
            isWalking = false;
            hasWaved = true;
        }
    }

    // Este método se llama al final de la animación "Waving"
    public void ResumeWalking()
    {
        isWalking = true;
        animator.SetBool("isWalking", true);
    }
}
