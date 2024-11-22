using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

namespace Contents {

    /// <summary>
    /// Definicion del tipo de contenido Walks, que contiene una lista de PathPhotos.
    /// </summary>
    public class Walk : Content {
        public List<Photo> photos; // Lista de PathPhotos
        private int currentPhotoIndex; // Índice de la foto actual

        public Walk() {
            photos = new List<Photo>();
            currentPhotoIndex = 0;
        }

        // Start is called before the first frame update
        void Start() {
            //or else
        }

        // Update is called once per frame
        void Update() {

        }

        public override void play(Dictionary<string, object> config) {
            base.play(config);

            if (photos.Count > 0) {
                // Reproduce la primera foto
                playCurrentPhoto();
            }
        }

        private void playCurrentPhoto() {
            if (SceneManager.GetActiveScene().name != "SphereScene") {
                SceneManager.LoadScene("SphereScene");
            }

            // Espera a que cargue la escena, luego reproduce la foto actual
            StartCoroutine(waitForSceneLoadThenPlay("SphereScene"));
        }

        private IEnumerator waitForSceneLoadThenPlay(string sceneName) {
            while (SceneManager.GetActiveScene().name != sceneName) {
                yield return null;
            }

            if (SceneManager.GetActiveScene().name == sceneName) {
                GameObject sphere = ContentManager.Instance.getSphere();
                MeshRenderer meshR = sphere.GetComponent<MeshRenderer>();
                Material[] meshmaterials = meshR.materials;
                meshmaterials[0] = photos[currentPhotoIndex].imageMaterial;
                meshR.materials = meshmaterials;
                GameObject player = GameObject.FindWithTag("Player");
                player.transform.position = sphere.transform.position;
            }
        }

        public void next() {
            if (photos.Count > 0) {
                currentPhotoIndex = (currentPhotoIndex + 1) % photos.Count;
                playCurrentPhoto();
            }
        }

        public void previous() {
            if (photos.Count > 0) {
                currentPhotoIndex = (currentPhotoIndex - 1 + photos.Count) % photos.Count;
                playCurrentPhoto();
            }
        }

        /// <summary>
        /// Agrega un objeto PathPhoto a la lista de fotos.
        /// </summary>
        /// <param name="photo"></param>
        public void addPhoto(Photo photo) {
            if (photo != null) {
                photos.Add(photo);
            }
        }

        /// <summary>
        /// Agrega el nombre del contenido a un Diccionario que contiene listas de nombres de contenidos, bajo la key "walks".
        /// </summary>
        /// <param name="dictio"></param>
        public override void addToDictionary(Dictionary<string, List<string>> dictio) {
            if (!dictio.ContainsKey("walks")) {
                dictio.Add("walks", new List<string>());
            }
            dictio["walks"].Add(this.nombre);
        }

        public Photo getFirst(){
            return photos[0];
        }

        public void setThumbnail(Texture2D textura){
            this.thumbnail =  textura;
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
            // TODO: Aca hace falta revisar las condiciones, y si se debe permitir personalizar algo mas
            bool hasTags = cambios.ContainsKey("tags");
            bool shouldSave = saveChange && (hasNewName || hasNewAudio || hasTags);
            return base.customize(cambios, audios, shouldSave);    
        }
    }
}
