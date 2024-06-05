using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.IO;
using System;

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
                
                if (this.soundContainer != null)
                {
                    if (video.audioTrackCount > 0)
                    {
                        videoPlayer.SetDirectAudioVolume(0, 0);
                    }
                }
                else
                {
                    videoPlayer.SetDirectAudioVolume(0, 1);;
                }
                // Creo un rendertexture a partir de la resolucion de video, y le doy play
                videoPlayer.prepareCompleted += this.getVideoSizeAndPlay;
                videoPlayer.Prepare();
                meshR.materials = meshmaterials;
                player.transform.position = sphere.transform.position;
            }
            isPaused = false;
        }
        
        public override bool delete() {
            if (thumbnail != null && thumbnail != Texture2D.whiteTexture) {
                UnityEngine.Object.Destroy(thumbnail);
            }
            string disablesFileLocation = $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}disables.txt";
            if (!File.Exists(disablesFileLocation)) {
                try {
                    File.Create(disablesFileLocation).Dispose();
                    using (StreamWriter streamWriter = File.AppendText(disablesFileLocation)) {
                        streamWriter.Write(this.nombre);
                    }
                    return true;
                } catch (Exception e) {
                    Debug.Log(e.StackTrace);
                    return false;
                }
            } else {
                try {
                    using (StreamWriter streamWriter = File.AppendText(disablesFileLocation)) {
				        streamWriter.Write("\n"+this.nombre); 
			        }
                    return true;
                } catch (Exception e) {
                    Debug.Log(e.StackTrace);
                    return false;
                }
            }
        }
    }
}