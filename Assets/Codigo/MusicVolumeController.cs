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
    public float stillnessDelay = 4f; // Time in seconds before volume decreases when user stops moving
    public MetricaDecrementoSonido metricaDecrementoSonido;

    private AudioSource audioSource;
    private Vector3 lastPosition;
    private float currentVolume;
    private float stillnessTimer = 0f;
    
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

        if (metricaDecrementoSonido == null)
        {
            Debug.LogError("MetricaDecrementoSonido not assigned in Inspector on " + gameObject.name);
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
            stillnessTimer = 0f; // Reset timer when sparrow sings
        } else {
            //public static float Distance(Vector3 a, Vector3 b); - Returns the distance between a and b.
            float distanceMoved = Vector3.Distance(user.position, lastPosition);
            bool isUserMoving = distanceMoved > stillnessThreshold;
            
            if (isUserMoving)
            {
                targetVolume = maxVolume;
                stillnessTimer = 0f; // Reset timer when user moves
            }
            else
            {
                stillnessTimer += Time.deltaTime;
                // Only decrease volume after the delay period
                if (stillnessTimer >= stillnessDelay)
                {
                    targetVolume = minVolume;
                    // Notify the metric when volume decreases due to stillness
                    metricaDecrementoSonido?.OnVolumeDecrease();
                }
                else
                {
                    targetVolume = maxVolume; // Keep max volume during the delay
                }
            }
        }

        currentVolume = Mathf.Lerp(currentVolume, targetVolume, volumeChangeSpeed * Time.deltaTime);
        audioSource.volume = currentVolume;
        lastPosition = user.position;
    }
}
