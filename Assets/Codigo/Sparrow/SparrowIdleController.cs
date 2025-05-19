using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparrowIdleController : MonoBehaviour
{
    public Animator animator;
    public float minInterval = 2f;
    public float maxInterval = 4f;

    private float timer = 0f;
    private float nextActionTime;
    
    // Start is called before the first frame update
    void Start()
    {
        SetNextActionTime();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= nextActionTime)
        {
            // int Range, [minInclusive..maxExclusive)
            int choice = Random.Range(0, 2); // 0: left, 1: right

            if (choice == 0)
            {
                animator.SetTrigger("Trigger_LookLeft");
            }
            else
            {
                animator.SetTrigger("Trigger_LookRight");
            }

            timer = 0f;
            SetNextActionTime();
        }
    }

    // time until the sparrow moves its head again
    void SetNextActionTime()
    {
        // float Range, [minInclusive..maxInclusive] (range is inclusive)
        nextActionTime = Random.Range(minInterval, maxInterval);
    }
}
