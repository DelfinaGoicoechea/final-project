using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        public override void play() {
            base.play();

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
    }
}
