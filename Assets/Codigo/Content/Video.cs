using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

namespace Contents{

    /// <summary>
    /// Definicion del tipo de contenido video
    /// </summary>
    public class Video : Content
    {
        public VideoClip video;
        public Material material;
        public RenderTexture renderTexture;
        public bool isEquiangularCubemap;
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
            // Pauso audio que se haya configurado al ver una imagen
            AudioSource audioSource = ContentManager.Instance.audioSource;
            audioSource.Pause();
            // Siempre las imagenes y videos se reproducen en la escena de esferas
            if (SceneManager.GetActiveScene().name != "SphereScene")
            {
                SceneManager.LoadScene("SphereScene");
            }
            // Espero a que carge la escena, para no tener problemas al buscar la esfera
            StartCoroutine(waitForSceneLoadThenPlay("SphereScene", audioSource));
        }

        /// <summary>
        /// Agrega el nombre del contenido a un Diccionario que
        ///  contiene listas de nombres de contenidos, bajo la key
        ///  "videos".
        /// </summary>
        /// <param name="dictio"></param>
        public override void addToDictionary(Dictionary<string, List<string>> dictio){
            if (!dictio.ContainsKey("videos")){
                dictio.Add("videos", new List<string>());
            }
            dictio["videos"].Add(this.nombre);
        }

        /// <summary>
        /// Pauses the video, and any music associated with it the scene.
        /// </summary>
        public override void pause()
        {
            base.pause();
            VideoPlayer videoPlayer = GameObject.FindWithTag("VideoPlayer").GetComponent<VideoPlayer>();
            videoPlayer.Pause();
            isPaused = true;
        }

        public override void stop()
        {
            base.stop();
            VideoPlayer videoPlayer = GameObject.FindWithTag("VideoPlayer").GetComponent<VideoPlayer>();
            videoPlayer.Stop();
            isPaused = false;
        }

        private IEnumerator waitForSceneLoadThenPlay(string sceneName, AudioSource audioSource)
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