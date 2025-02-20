using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

namespace Contents{

    /// <summary>
    /// Definicion del tipo de contenido video
    /// </summary>
    public class Photo : Content
    {
        public Material imageMaterial;

        // Start is called before the first frame update
        void Start()
        {
            //or else
        }

        // Update is called once per frame
        void Update()
        {

        }
        public override void play(Dictionary<string, object> config)
        {
            // Pauso audio que se haya configurado al ver una imagen
            AudioSource audioSource = ContentPlayer.Instance.audioSource;
            audioSource.Pause();
            // Siempre las imagenes y videos se reproducen en la escena de esferas
            if (SceneManager.GetActiveScene().name != "SphereScene")
            {
                SceneManager.LoadScene("SphereScene");
            }
            // Espero a que carge la escena, para no tener problemas al buscar la esfera
            StartCoroutine(waitForSceneLoadThenPlay("SphereScene", audioSource, config));
        }

        private IEnumerator waitForSceneLoadThenPlay(string sceneName, AudioSource audioSource, Dictionary<string, object> config)
        {
            // Busy wait, pero el delay que debo esperar realmente es minimo
            while (SceneManager.GetActiveScene().name != sceneName)
            {
                yield return null;
            }

            if (isPaused)
            {
                isPaused = false;
                if (this.soundContainer != null){
                    audioSource.Play();
                }
                yield break;
            }

            // Inserto el render definido a la esfera y transporto al player dentro
            if(SceneManager.GetActiveScene().name == sceneName)
            {
                GameObject sphere = ContentManager.Instance.getSphere();
                MeshRenderer meshR = sphere.GetComponent<MeshRenderer>();
                Material[] meshmaterials = meshR.materials;
                meshmaterials[0] = imageMaterial;
                meshR.materials = meshmaterials;
                sphere.transform.eulerAngles = Vector3.zero; // Acomodo la rotacion de la esfera, por si la cambio algun video
            }
            base.play(config);
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

        public Texture2D getThumbnail(){
            return thumbnail;
        }

        public override bool delete() {
            Resources.UnloadAsset(imageMaterial);
            if (thumbnail != null && thumbnail != Texture2D.whiteTexture) {
                Resources.UnloadAsset(thumbnail);
            }
            string disablesFileLocation = $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}disables.txt";
            if (!File.Exists(disablesFileLocation)) {
                try {
                    File.Create(disablesFileLocation).Dispose();
                    using (StreamWriter streamWriter = File.AppendText(disablesFileLocation)) {
                        streamWriter.Write(this.nombre);
                    }
                    return base.delete();
                } catch (Exception e) {
                    Debug.Log(e.StackTrace);
                    return false;
                }
            } else {
                // si el archivo existe, pero esta vacio, no meto salto de linea al inicio
                long fileLength = new FileInfo(disablesFileLocation).Length;
                string toWrite = (fileLength == 0) ? this.nombre : "\n"+this.nombre;
                try {
                    using (StreamWriter streamWriter = File.AppendText(disablesFileLocation)) {
                        streamWriter.Write(toWrite);
			        }
                    return base.delete();
                } catch (Exception e) {
                    Debug.Log(e.StackTrace);
                    return false;
                }
            }
        }

        public override string customize(Dictionary<string, object> cambios, List<AudioContainer> audios, bool saveChange){
            bool hasNewName = cambios.ContainsKey("newName");
            bool hasNewAudio = cambios.ContainsKey("newAudio");
            bool shouldSave = saveChange && (hasNewName || hasNewAudio);
            return base.customize(cambios, audios, shouldSave);    
        }
    }
}