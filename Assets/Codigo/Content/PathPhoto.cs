using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System;

namespace Contents{
    /// <summary>
    /// Definicion del tipo de contenido video
    /// </summary>
    public class PathPhoto : Photo
    {
        public string imagePath;
        private bool hasThumbnail = false;

        // Start is called before the first frame update
        void Start()
        {
            Shader shader = Shader.Find("Skybox/Panoramic");
            imageMaterial = new Material(shader);
            string thumbnailLocation = $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}thumbnails{Path.DirectorySeparatorChar}{this.nombre}.png";
            if(File.Exists(thumbnailLocation)){
                byte[] bytes = File.ReadAllBytes(thumbnailLocation);
                Texture2D loadTexture = new Texture2D(1, 1, TextureFormat.RGB24, false);
                ImageConversion.LoadImage(loadTexture, bytes);
                this.thumbnail = loadTexture;
                hasThumbnail = true;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
        public override void play()
        {
            // --------------- Cargar Textura ---------------
            byte[] bytes = File.ReadAllBytes(imagePath);
            Texture2D loadTexture = new Texture2D(1, 1, TextureFormat.RGB24, false);
            ImageConversion.LoadImage(loadTexture, bytes);
            //loadTexture.Compress(false);

            // --------------- Cargar Material ---------------
            imageMaterial.mainTexture = loadTexture;
            imageMaterial.renderQueue = 2450; // Hace que no se superpongan las imagenes de las esferas
            base.play();
            if(!hasThumbnail){
                StartCoroutine(captureThumbnail());
            }
        }

        public override void stop(){
            UnityEngine.Object.Destroy(imageMaterial.mainTexture);
            imageMaterial.mainTexture = null;
        }

        private IEnumerator captureThumbnail(){
            while (SceneManager.GetActiveScene().name != "SphereScene")
            {
                yield return new WaitForSeconds(1);
            }
            Texture2D image = ThumbnailCreator.Capture(this.nombre);
            this.thumbnail = image;
            this.hasThumbnail = true;
        }

        public override bool delete () {
            // delete imagen
            try {
                File.Delete(imagePath);
            } catch (Exception e) {
                Debug.Log(e.StackTrace);
                return false;
            }
            // delete thumbnail}
            if (hasThumbnail) {
                char OsSep = Path.DirectorySeparatorChar;
                string thumbLocation = $"{Application.persistentDataPath}{OsSep}thumbnails{OsSep}{nombre}.png";
                try {
                    File.Delete(thumbLocation);
                } catch (Exception e) {
                    Debug.Log(e.StackTrace);
                    return false;
                }
            }
            return true;
        }
    }
}