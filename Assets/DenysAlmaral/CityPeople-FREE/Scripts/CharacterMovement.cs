using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private Animator animator;
    private bool isWalking = true;
    public float speed = 2f;
    private float walkTime = 0f;
    private bool hasWaved = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("isWalking", true);
    }

    void Update()
    {
        if (isWalking)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            walkTime += Time.deltaTime;

            // Después de 3 segundos, detenerse y saludar
            if (walkTime >= 5f && !hasWaved)
            {
                isWalking = false;
                animator.SetBool("isWalking", false);
                animator.SetTrigger("wave");
                hasWaved = true;
            }
        }
    }

    // Este método se llama al final de la animación "Waving"
    public void ResumeWalking()
    {
        isWalking = true;
        animator.SetBool("isWalking", true);
    }
}
