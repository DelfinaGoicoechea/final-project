using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

namespace Contents{

    /// <summary>
    /// Definicion del tipo de contenido video
    /// </summary>
    public class ConcreteVideo : Video
    {
        public VideoClip video;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        protected override IEnumerator waitForSceneLoadThenPlay(string sceneName, AudioSource audioSource)
        {
            // Busy wait, pero el delay es que debo esperar realmente es minimo
            while (SceneManager.GetActiveScene().name != sceneName)
            {
                yield return null;
            }

            if (SceneManager.GetActiveScene().name == sceneName)
            {
                GameObject sphere;
                ContentManager cm = ContentManager.Instance;
                sphere = (isEquiangularCubemap) ? cm.getSphereEAC() : cm.getSphere();

                // Asigno el material a la esfera
                MeshRenderer meshR = sphere.GetComponent<MeshRenderer>();
                Material[] meshmaterials = meshR.materials;
                meshmaterials[0] = material;

                VideoPlayer videoPlayer = GameObject.FindWithTag("VideoPlayer").GetComponent<VideoPlayer>();
                GameObject player = GameObject.FindWithTag("Player");

                if (isPaused)
                {
                    videoPlayer.Play();
                    isPaused = false;
                    if (this.soundContainer != null){
                        audioSource.Play();
                    }
                    yield break;
                }
                
                // Asigno RenderTexture y video al VideoPlayer
                videoPlayer.Stop();
                videoPlayer.url = null;
                videoPlayer.clip = video;
                videoPlayer.targetTexture = renderTexture;

                if (this.soundContainer != null)
                {
                    if (video.audioTrackCount > 0)
                    {
                        videoPlayer.SetDirectAudioVolume(0, 0);
                    }
                    base.play();
                }
                else
                {
                    videoPlayer.SetDirectAudioVolume(0, 1);;
                }
                videoPlayer.Play();
                meshR.materials = meshmaterials;
                player.transform.position = sphere.transform.position;
            }
            isPaused = false;
        }
    }
}