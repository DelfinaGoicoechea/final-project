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
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
        public override void play(Dictionary<string, object> config)
        {
            // --------------- Cargar Textura ---------------
            byte[] bytes = File.ReadAllBytes(imagePath);
            Texture2D loadTexture = new Texture2D(1, 1, TextureFormat.RGB24, false);
            ImageConversion.LoadImage(loadTexture, bytes);
            //loadTexture.Compress(false);

            // --------------- Cargar Material ---------------
            imageMaterial.mainTexture = loadTexture;
            imageMaterial.renderQueue = 2450; // Hace que no se superpongan las imagenes de las esferas
            base.play(config);
            if(thumbnail == null || thumbnail == Texture2D.whiteTexture){
                StartCoroutine(captureThumbnail());
            }
        }

        public override void stop(){
            base.stop();
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
        }

        public override bool delete () {
            if (thumbnail != null && thumbnail != Texture2D.whiteTexture) {
                UnityEngine.Object.Destroy(thumbnail);
            }
            // delete imagen
            try {
                File.Delete(imagePath);
            } catch (Exception e) {
                Debug.Log(e.StackTrace);
                return false;
            }
            // delete thumbnail}
            if (thumbnail != null && thumbnail != Texture2D.whiteTexture) {
                char OsSep = Path.DirectorySeparatorChar;
                string thumbLocation = $"{Application.persistentDataPath}{OsSep}thumbnails{OsSep}{nombre}.png";
                try {
                    File.Delete(thumbLocation);
                } catch (Exception e) {
                    Debug.Log(e.StackTrace);
                    return false;
                }
            }
            return base.delete();
        }

        public override bool changeNombre(string newName){
            char OsSep = Path.DirectorySeparatorChar;
            string oldThPath = $"{Application.persistentDataPath}{OsSep}thumbnails{OsSep}{nombre}.png";
            string newThPath = $"{Application.persistentDataPath}{OsSep}thumbnails{OsSep}{newName}.png";

            // Cambio el nombre del archivo de thumbnail en caso de que exista
            if (thumbnail != null && thumbnail != Texture2D.whiteTexture) {
                if (File.Exists(oldThPath)) {
                    try {
                        File.Move(oldThPath, newThPath);
                    } catch (Exception e) {
                        Debug.LogWarning(e.StackTrace);
                        return false;
                    }
                }
            } else {
                // Puede que no tenga thumbnail, pero solo porque se cambio el nombre del contenido
                // respecto del nombre original.
                // Entonces al buscar bajo nombre no se encontro un thumbnail, ya que aparece bajo el
                // nombre cambiado.
                // Esto surge de "ContentManager -> LoadCustomization()"
                if (File.Exists(newThPath)) {
                    byte[] bytes = File.ReadAllBytes(newThPath);
                    Texture2D loadTexture = new Texture2D(1, 1, TextureFormat.RGB24, false);
                    ImageConversion.LoadImage(loadTexture, bytes);
                    this.thumbnail = loadTexture;
                }
            }
            this.nombre = newName;
            return true;
        }
    }
}