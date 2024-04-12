using System.IO;
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
    public class PathPhoto : Photo
    {
        public string imagePath;

        // Start is called before the first frame update
        void Start()
        {
            Shader shader = Shader.Find("Skybox/Panoramic");
            imageMaterial = new Material(shader);
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
        }

        public override void stop(){
            //TODO: Eliminar Imagen de RAM
            Object.Destroy(imageMaterial.mainTexture);
            imageMaterial.mainTexture = null;
        }

        //TODO: Actualizar Thumbnail al tomar foto de la escena en su primera carga
    }
}