using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

namespace Contents{

    /// <summary>
    /// Definicion del tipo de contenido video
    /// </summary>
    public abstract class Video : Content
    {
        public Material material;
        public bool isEquiangularCubemap;
        protected static Vector3 noRotation = Vector3.zero;
        protected static Vector3 eacRotation = new Vector3(0.0f, 0.0f, 90.0f);

        /// <summary>
        /// Se obtiene la resolucion del video a partir de una textura temporal 
        /// del VideoPlayer, a partir de eso se crea una RenderTexture, se asigna 
        /// tanto como target del videoPlayer, como al material propio de Content.
        ///  "videos".
        /// </summary>
        /// <param name="videoPlayer"></param>
        protected void getVideoSizeAndPlay(VideoPlayer videoPlayer){
            int width = videoPlayer.texture.width;
            int height = videoPlayer.texture.height;
            RenderTexture rendTex = new RenderTexture(width, height, 16, RenderTextureFormat.ARGB32);
            videoPlayer.targetTexture = rendTex;
            this.material.mainTexture = rendTex;
            videoPlayer.Play();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void play(Dictionary<string, object> config)
        {
            if (isPaused)
            {
                VideoPlayer videoPlayer = GameObject.FindWithTag("VideoPlayer").GetComponent<VideoPlayer>();
                base.play(config);
                videoPlayer.Play();
                isPaused = false;
                return;
            }
            // Siempre las imagenes y videos se reproducen en la escena de esferas
            if (SceneManager.GetActiveScene().name != "SphereScene")
            {
                SceneManager.LoadScene("SphereScene");
            }

            AudioSource audioSource = ContentPlayer.Instance.audioSource;
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
        }

        public override void stop()
        {
            base.stop();
            VideoPlayer videoPlayer = GameObject.FindWithTag("VideoPlayer").GetComponent<VideoPlayer>();
            videoPlayer.Stop();
            Object.Destroy(material.mainTexture);
            material.mainTexture = null;
        }

        public override Dictionary<string,object> getCurrentConfig(){
            Dictionary<string,object> config = base.getCurrentConfig();
            config["isEAC"] = isEquiangularCubemap;
            return config;
        }

        public override string customize(Dictionary<string, object> cambios, List<AudioContainer> audios, bool saveChange = true){
            bool hasNewName = cambios.ContainsKey("newName");
            bool hasNewAudio = cambios.ContainsKey("newAudio");
            bool changedRepresentation = cambios.ContainsKey("isEquiangular");
			if (changedRepresentation){
                bool isEAC = (bool) cambios["isEquiangular"];
				this.isEquiangularCubemap = isEAC;
                if (isEAC){
                    material = (Material)Resources.Load("360_EAC", typeof(Material));
                }else{
                    material = (Material)Resources.Load("360_Material", typeof(Material));
                }
			}

            bool shouldSave = saveChange && (hasNewName || hasNewAudio || changedRepresentation);
            return base.customize(cambios, audios, saveChange);
        }

        protected void playAudioSource(){
            base.play(new Dictionary<string,object>());
        }
        protected abstract IEnumerator waitForSceneLoadThenPlay(string sceneName, AudioSource audioSource);
    }
}