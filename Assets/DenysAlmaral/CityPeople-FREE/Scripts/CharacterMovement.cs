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
            Debug.Log("Se empezo a saludar");
        }
    }

    // Este método se llama al final de la animación "Waving"
    public void ResumeWalking()
    {
        isWalking = true;
        animator.SetBool("isWalking", true);
        Debug.Log("Se empezo a caminar");
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.tag);
        if(collision.gameObject.tag == "hedge")
        {
            isWalking = false;
            animator.SetBool("isWalking", false);
        }
    }
}
