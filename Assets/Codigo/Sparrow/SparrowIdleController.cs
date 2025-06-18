using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparrowIdleController : MonoBehaviour
{
    private Animator animator;
    public Transform user; //reference to the camera transform
    public float rotationSpeed = 2f;

    private AudioSource audioSource;

    public float singRepeatInterval = 10f;

    public float minHeadInterval = 4f;
    public float maxHeadInterval = 8f;
    public float minFlapInterval = 3f;
    public float maxFlapInterval = 6f;

    private float singTimer = 0f;
    private float headMoveTimer = 0f;
    private float flapTimer = 0f;
    private float nextHeadMoveTime;
    private float nextFlapTime;

    private bool userInZone = false;
    private bool isSinging = false;
    

    //Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (animator == null)
        {
            Debug.LogError("Animator not assigned in Inspector on " + gameObject.name);
        }

        if (audioSource == null)
        {
            Debug.LogError("AudioSource not assigned in Inspector on " + gameObject.name);
        }

        SetNextHeadMoveTime();
        SetNextFlapTime();
    }


    //Update is called once per frame
    void Update()
    {
        //idle while user not in zone
        if (!userInZone)
        {
            headMoveTimer += Time.deltaTime;

            if (headMoveTimer >= nextHeadMoveTime)
            {
                //int Range, [minInclusive..maxExclusive)
                int choice = Random.Range(0, 2); // 0: left, 1: right

                //if choice is 0, set the trigger to the left, else set the trigger to the right
                animator.SetTrigger(choice == 0 ? "Trigger_LookLeft" : "Trigger_LookRight");

                headMoveTimer = 0f;
                SetNextHeadMoveTime();
            }
        }
        else //user in zone
        {
            //follows user position
            RotateTowardsUser();

            flapTimer += Time.deltaTime;
            singTimer += Time.deltaTime;

            //flap randomly
            if(flapTimer >= nextFlapTime)
            {
                animator.SetTrigger("Trigger_Fly");
                flapTimer = 0f;
                SetNextFlapTime();
            }

            //repeat singing after 'singRepeatInterval' time
            if(singTimer >= singRepeatInterval)
            {
                animator.SetTrigger("Trigger_Sing");
                singTimer = 0f;
            }

            //check if singing finished
            if(isSinging && !audioSource.isPlaying)
            {
                animator.SetTrigger("Trigger_SingFinished");
                isSinging = false;
            }
        }
    }


    //time until the sparrow moves its head again
    void SetNextHeadMoveTime()
    {
        // float Range, [minInclusive..maxInclusive] (range is inclusive)
        nextHeadMoveTime = Random.Range(minHeadInterval, maxHeadInterval);
    }


    //time until the sparrow flaps again
    void SetNextFlapTime()
    {
        //float Range, [minInclusive..maxInclusive] (range is inclusive)
        nextFlapTime = Random.Range(minFlapInterval, maxFlapInterval);
    }


    public void OnUserEnterZone()
    {
        userInZone = true;
        animator.SetTrigger("Trigger_Sing");
        flapTimer = 0f;
        singTimer = 0f;
        SetNextFlapTime();
    }


    public void OnUserExitZone()
    {
        isSinging = false;
        userInZone = false;
        animator.SetTrigger("Trigger_Reset");
    }


    private void RotateTowardsUser()
    {
        if (user == null)
            return;

        //Get direction from sparrow to user
        Vector3 direction = user.position - transform.position;
        direction.y = 0f; //keep horizontal rotation only

        //Calculate rotation to look at user
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }  

    public void PlaySingSound()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
            isSinging = true;
        }
    }

    public bool IsSinging()
    {
        return isSinging;
    }
}
