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
            ContentManager cm = ContentManager.Instance;
            GameObject sphere = (equiangularCubemap) ? cm.getSphereEAC() : cm.getSphere();
            
            VideoPlayer videoPlayer = GameObject.FindWithTag("VideoPlayer").GetComponent<VideoPlayer>();
            GameObject player = GameObject.FindWithTag("Player");
           
            if (isPaused)
            {
                videoPlayer.Play();
                isPaused = false;
                if (this.sound != null){
                    audioSource.Play();
                }
                return;
            }

            // Asigno el material para la esfera
            MeshRenderer meshR = sphere.GetComponent<MeshRenderer>();
            Material[] meshmaterials = meshR.materials;
            meshmaterials[0] = material;

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
            videoPlayer.Prepare();
            videoPlayer.Play();
            meshR.materials = meshmaterials;
            player.transform.position = sphere.transform.position;
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
            GameObject sphere;
            ContentManager cm = ContentManager.Instance;
            sphere = (equiangularCubemap) ? cm.getSphereEAC() : cm.getSphere();

            // Asigno el material a la esfera
            MeshRenderer meshR = sphere.GetComponent<MeshRenderer>();
            Material[] meshmaterials = meshR.materials;
            meshmaterials[0] = material;

            VideoPlayer videoPlayer = GameObject.FindWithTag("VideoPlayer").GetComponent<VideoPlayer>();
            GameObject player = GameObject.FindWithTag("Player");
            
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
            meshR.materials = meshmaterials;
            player.transform.position = sphere.transform.position;
        }
    }
}