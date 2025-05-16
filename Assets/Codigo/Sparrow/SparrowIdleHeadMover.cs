using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparrowIdleHeadMover : MonoBehaviour
{
    public Animator animator;
    public string parameterName = "IdleSwitch";
    public float switchInterval = 3f;

    private float timer = 0f;

    void Start()
    {
        if (animator == null)
        {
            Debug.LogError("Animator not assigned in Inspector on " + gameObject.name);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= switchInterval)
        {
            // int function of Range is [minInclusive..maxExclusive)
            int randomIdle = Random.Range(0, 3); // 0, 1, 2
            animator.SetFloat(parameterName, randomIdle);
            timer = 0f;
        }
    }
}
