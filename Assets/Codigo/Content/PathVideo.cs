using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System;

namespace Contents{

    /// <summary>
    /// Definicion del tipo de contenido video
    /// </summary>
    public class PathVideo : Video
    {
        public string videoFilePath;
        protected bool hasThumbnail = false;

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
                videoPlayer.clip = null;
                videoPlayer.url = $"file://{videoFilePath}";
                videoPlayer.targetTexture = renderTexture;

                if (this.soundContainer != null)
                {
                    try
                    {
                        videoPlayer.SetDirectAudioVolume(0, 0);
                    }
                    catch (Exception err)
                    {
                        Debug.Log($"{err.Message}, {err.StackTrace}");
                    }
                    base.play();
                }
                else
                {
                    try
                    {
                        videoPlayer.SetDirectAudioVolume(0, 1);
                    }
                    catch (Exception err)
                    {
                        Debug.Log($"{err.Message}, {err.StackTrace}");
                    }
                }
                videoPlayer.Play();
                meshR.materials = meshmaterials;
                player.transform.position = sphere.transform.position;
            }
            isPaused = false;
            if(!hasThumbnail){
                StartCoroutine(captureThumbnail());
            }
        }

        protected IEnumerator captureThumbnail(){
            VideoPlayer videoPlayer = GameObject.FindWithTag("VideoPlayer").GetComponent<VideoPlayer>();
            while (!videoPlayer.isPlaying)
            {
                yield return new WaitForSeconds(1);
            }
            Texture2D image = ThumbnailCreator.Capture(this.nombre);
            this.thumbnail = image;
            this.hasThumbnail = true;
        }
    }
}