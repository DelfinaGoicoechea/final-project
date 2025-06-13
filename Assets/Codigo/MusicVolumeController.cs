using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicVolumeController : MonoBehaviour
{
    public SparrowIdleController sparrowController;
    public Transform user;
    public float minVolume = 0.2f;
    public float maxVolume = 1f;
    //Threshold to determine if the user is moving or not
    public float stillnessThreshold = 0.01f;
    public float volumeChangeSpeed = 1f;
    public float sparrowSingVolume = 0.05f;

    private AudioSource audioSource;
    private Vector3 lastPosition;
    private float currentVolume;
    
    // Start is called before the first frame update
    void Start()
    {
        if (user == null)
        {
            Debug.LogError("User not assigned in Inspector on " + gameObject.name);
        }

        if (sparrowController == null)
        {
            Debug.LogError("Sparrow Controller not assigned in Inspector on " + gameObject.name);
        }

        audioSource = GetComponent<AudioSource>();
        lastPosition = user.position;
        currentVolume = audioSource.volume;
    }

    // Update is called once per frame
    void Update()
    {
        float targetVolume;

        if (sparrowController.IsSinging())
        {
            targetVolume = sparrowSingVolume;
        } else {
            //public static float Distance(Vector3 a, Vector3 b); - Returns the distance between a and b.
            float distanceMoved = Vector3.Distance(user.position, lastPosition);
            bool isUserMoving = distanceMoved > stillnessThreshold;
            targetVolume = isUserMoving ? maxVolume : minVolume;  
        }

        currentVolume = Mathf.Lerp(currentVolume, targetVolume, volumeChangeSpeed * Time.deltaTime);
        audioSource.volume = currentVolume;
        lastPosition = user.position;
    }
}
