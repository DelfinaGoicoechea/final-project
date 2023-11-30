using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class Video : Content
{
    public VideoClip video;
    public Material material;
    public RenderTexture renderTexture;
    public bool equiangularCubemap;
    private bool isPaused;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void play()
    {

        isPaused = false;

        // Pauso audio que se haya configurado al ver una imagen
        AudioSource audioSource = ContentManager.Instance.audioSource;
        audioSource.Pause();

        if (SceneManager.GetActiveScene().name == "SphereScene")
        {
            GameObject sphere = GameObject.FindWithTag("ProjectionSphere");
            
            VideoPlayer videoPlayer = GameObject.FindWithTag("VideoPlayer").GetComponent<VideoPlayer>();
           
            if (isPaused)
            {
                videoPlayer.Play();
                isPaused = false;
                if (this.sound != null){
                    audioSource.Play();
                }
                return;
            }
            
            // Roto la esfera dependiendo de si es o no un Equi-Angular Cubemap
            StartCoroutine(checkRotation(sphere));

            // Asigno el material a la esfera
            MeshRenderer meshR = sphere.GetComponent<MeshRenderer>();
            Material[] meshmaterials = meshR.materials;
            meshmaterials[0] = material;
            meshR.materials = meshmaterials;

            // Asigno RenderTexture y video al VideoPlayer
            videoPlayer.Stop();
            videoPlayer.clip = video;
            videoPlayer.targetTexture = renderTexture;
            if (this.sound != null)
            {
                if (video.audioTrackCount > 0)
                {
                    videoPlayer.SetDirectAudioVolume(0, 0);
                }
                audioSource.clip = this.sound;
                audioSource.Play();
            }
            else
            {
                videoPlayer.SetDirectAudioVolume(0, 1);
            }
            videoPlayer.Play();
        }
        else
        {
            SceneManager.LoadScene("SphereScene");

            // Espero a que carge la escena, para no tener problemas al buscar la esfera
            if (SceneManager.GetActiveScene().name != "SphereScene")
            {
                StartCoroutine(waitForSceneLoad("SphereScene", audioSource));
            }
        }
    }

    public override void pause()
    {
        VideoPlayer videoPlayer = GameObject.FindWithTag("VideoPlayer").GetComponent<VideoPlayer>();
        AudioSource audioSource = ContentManager.Instance.audioSource;
        videoPlayer.Pause();
        if (this.sound != null){
            audioSource.Pause();
        }
        isPaused = true;
    }

    private IEnumerator waitForSceneLoad(string sceneName, AudioSource audioSource)
    {
        // Busy wait, pero el delay es que debo esperar realmente es m�nimo
        while (SceneManager.GetActiveScene().name != sceneName)
        {
            yield return null;
        }

        if (SceneManager.GetActiveScene().name == sceneName)
        {
            GameObject sphere = ContentManager.Instance.getSphere();

            // Roto la esfera dependiendo de si es o no un Equi-Angular Cubemap
            StartCoroutine(checkRotation(sphere));

            // Asigno el material a la esfera
            MeshRenderer meshR = sphere.GetComponent<MeshRenderer>();
            Material[] meshmaterials = meshR.materials;
            meshmaterials[0] = material;
            meshR.materials = meshmaterials;

            VideoPlayer videoPlayer = GameObject.FindWithTag("VideoPlayer").GetComponent<VideoPlayer>();
            
            // Asigno RenderTexture y video al VideoPlayer
            videoPlayer.Stop();
            videoPlayer.clip = video;
            videoPlayer.targetTexture = renderTexture;

            if (this.sound != null)
            {
                if (video.audioTrackCount > 0)
                {
                    videoPlayer.SetDirectAudioVolume(0, 0);
                }
                audioSource.clip = this.sound;
                audioSource.Play();
            }
            else
            {
                videoPlayer.SetDirectAudioVolume(0, 1);;
            }
            videoPlayer.Play();
        }
    }

    private IEnumerator checkRotation(GameObject sphere)
    {
        // obtengo la posicion y rotacion local de la esfera
        Vector3 position = new Vector3(0.0f, 1.0f, 0.0f);
        Quaternion rotation = new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);
        sphere.transform.GetLocalPositionAndRotation(out position, out rotation);

        // Roto la esfera si es un Equi-Angular Cubemap (Porque no se c�mo rotarlo desde el shader)
        Quaternion rotated = new Quaternion(0.00000f, 0.00000f, 0.70711f, 0.70711f);
        if (equiangularCubemap && rotation != rotated)
        {
            sphere.transform.Rotate(0.0f, 0.0f, 90.0f);
        }
        else
        {
            //Acomodo la esfera si ya estaba rotada y este video NO es un Equi-Angular Cubemap
            if (!equiangularCubemap && rotation == rotated)
            {
                sphere.transform.Rotate(0.0f, 0.0f, -90.0f);
            }
        }
        yield return null;
    }
}