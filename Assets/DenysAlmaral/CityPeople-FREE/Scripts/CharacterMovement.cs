using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private Animator animator;
    private bool isWalking = false;
    public float speed = 2f;
    private float idleTime = 0f;
    private bool hasWaved = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        // El personaje arranca quieto en Idle, sin necesidad de activar nada aún
    }

    void Update()
    {
        // Acumulamos el tiempo desde el arranque
        idleTime += Time.deltaTime;

        // A los 5 segundos, saluda (una sola vez)
        if (!hasWaved && idleTime >= 5f)
        {
            animator.SetTrigger("wave");
            hasWaved = true;
        }

        // Si está caminando, lo movemos hacia adelante
        if (isWalking)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    // Este método debe ser llamado al final de la animación de saludo (via Animation Event)
    public void ResumeWalking()
    {
        isWalking = true;
    }
}
