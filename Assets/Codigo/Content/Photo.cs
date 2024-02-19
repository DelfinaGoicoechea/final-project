using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

namespace Contents{

    /// <summary>
    /// Definicion del tipo de contenido video
    /// </summary>
    public class Photo : Content
    {
        public Material material;

        // Start is called before the first frame update
        void Start()
        {
            //or else
        }

        // Update is called once per frame
        void Update()
        {

        }
        public override void play()
        {
            AudioSource audioSource = ContentManager.Instance.audioSource;
            audioSource.Pause();

            // Si no estoy en la escena adecuada, me voy a ella
            // Siempre las imagenes y videos se reproducen en la escena de esferas
            if (SceneManager.GetActiveScene().name == "SphereScene")
            {
                VideoPlayer videoPlayer = GameObject.FindWithTag("VideoPlayer").GetComponent<VideoPlayer>();
                videoPlayer.Pause();

                GameObject sphere = ContentManager.Instance.getSphere();
                
                MeshRenderer meshR = sphere.GetComponent<MeshRenderer>();
                Material[] meshmaterials = meshR.materials;
                meshmaterials[0] = material;
                meshR.materials = meshmaterials;

                GameObject player = GameObject.FindWithTag("Player");
                player.transform.position = sphere.transform.position;

                if (this.sound != null)
                {
                    audioSource.clip = this.sound;
                    audioSource.Play();
                }
            }
            else
            {
                SceneManager.LoadScene("SphereScene");
                if (this.sound != null)
                {
                    audioSource.clip = this.sound;
                    audioSource.Play();
                }

                // Espero a que carge la escena, para no tener problemas al buscar la esfera
                if (SceneManager.GetActiveScene().name != "SphereScene")
                {
                    StartCoroutine(waitForSceneLoad("SphereScene"));
                }
            }
        }

        private IEnumerator waitForSceneLoad(string sceneName)
        {
            // Busy wait, pero el delay que debo esperar realmente es minimo
            while (SceneManager.GetActiveScene().name != sceneName)
            {
                yield return null;
            }

            // Inserto el render definido a la esfera y transporto al player dentro
            if(SceneManager.GetActiveScene().name == sceneName)
            {
                GameObject sphere = ContentManager.Instance.getSphere();
                MeshRenderer meshR = sphere.GetComponent<MeshRenderer>();
                Material[] meshmaterials = meshR.materials;
                meshmaterials[0] = material;
                meshR.materials = meshmaterials;
                GameObject player = GameObject.FindWithTag("Player");
                player.transform.position = sphere.transform.position;
            }
        }

        /// <summary>
        /// Agrega el nombre del contenido a un Diccionario que
        ///  contiene listas de nombres de contenidos, bajo la key
        ///  "photos".
        /// </summary>
        /// <param name="dictio"></param>
        public override void addToDictionary(Dictionary<string, List<string>> dictio){
            if (!dictio.ContainsKey("photos")){
                dictio.Add("photos", new List<string>());
            }
            dictio["photos"].Add(this.nombre);
        }
    }
}