using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicVolumeController : MonoBehaviour
{
    public SparrowIdleController sparrowController;
    public Transform user;
    public MetricaDecrementoSonido metricaDecrementoSonido;
    public MetricaDuracionInmovilidad metricaDuracionInmovilidad;
    public float minVolume = 0.2f;
    public float maxVolume = 1f;
    //Threshold to determine if the user is moving or not
    public float stillnessThreshold = 0.01f;
    public float volumeChangeSpeed = 1f;
    public float sparrowSingVolume = 0.05f;

    private AudioSource audioSource;
    private Vector3 lastPosition;
    private float currentVolume;
    private bool wasMoving = true;
    private float stillnessTimer = 0f; // Timer to track how long the user has been still
    public float stillnessDurationThreshold = 3f; // Duration (in seconds) the user must be still before metrics are called
    
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

        // Always track movement and stillness
        float distanceMoved = Vector3.Distance(user.position, lastPosition);
        bool isUserMoving = distanceMoved > stillnessThreshold;

        if (isUserMoving)
        {
            stillnessTimer = 0f;
            if (!wasMoving)
            {
                metricaDuracionInmovilidad?.OnStillnessEnd();
                wasMoving = true;
            }
        }
        else
        {
            stillnessTimer += Time.deltaTime;
            if (wasMoving && stillnessTimer >= stillnessDurationThreshold)
            {
                metricaDecrementoSonido?.OnVolumeDecrease();
                metricaDuracionInmovilidad?.OnStillnessStart();
                wasMoving = false;
            }
        }

        // Only the volume logic depends on sparrow singing
        if (sparrowController.IsSinging())
        {
            targetVolume = sparrowSingVolume;
        }
        else if (isUserMoving)
        {
            targetVolume = maxVolume;
        }
        else
        {
            targetVolume = minVolume;
        }

        currentVolume = Mathf.Lerp(currentVolume, targetVolume, volumeChangeSpeed * Time.deltaTime);
        audioSource.volume = currentVolume;
        lastPosition = user.position;
    }
}