using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Contents{
	/// <summary>
	/// Clase encargada de gestionar la creacion de nuevos contenidos.
	/// </summary>
    public class ContentCreator
    {

        private ContentManager cManager = GameObject.FindWithTag("ContentManager").GetComponent<ContentManager>();
        private static Texture2D CreateThumbnail(ref Material material, Vector2Int resolution, string filename = "")
        {
            RenderTexture renderTexture = RenderTexture.GetTemporary(resolution.x, resolution.y);
            Graphics.Blit(null, renderTexture, material);
            Texture2D texture = new Texture2D(resolution.x, resolution.y, TextureFormat.RGB24, false);
            texture.filterMode = FilterMode.Bilinear;
            texture.wrapMode = TextureWrapMode.Clamp;
            RenderTexture.active = renderTexture;
            texture.ReadPixels(new Rect(Vector2.zero, resolution), 0, 0);
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(renderTexture);
            return texture;
        }

        /// <summary>
	    /// Crea un contenido Photo a partir de el path a una imagen.
        /// La imagen debe ser de tipo jpg o png (al menos de momento).
        /// El GameObject asociado a la Photo, DEBE ser agregado a ContentManager,
        /// o se va a perder al cambiar de escena o ejecutar Stop.
	    /// </summary>
        public Photo CreatePhoto(FileInfo imageFileInfo, AudioContainer soundContainer = null){
            // --------------- Cargar Textura ---------------
            byte[] bytes = File.ReadAllBytes(imageFileInfo.FullName);
            Texture2D loadTexture = new Texture2D(1, 1, TextureFormat.RGB24, false);
            ImageConversion.LoadImage(loadTexture, bytes);
            loadTexture.Compress(false);

            // --------------- Cargar Material ---------------
            Shader shader = Shader.Find("Skybox/Panoramic");
            Material imageMaterial = new Material(shader);
            imageMaterial.mainTexture = loadTexture;
            imageMaterial.renderQueue = 2450; // Hace que no se superpongan las imagenes de las esferas

            // --------------- Crear Photo ---------------
            Texture2D thumbnail = CreateThumbnail(ref imageMaterial, new Vector2Int(512, 512));
            string filename = imageFileInfo.Name.Split('.')[0];
            

            // Se DEBE crear un GameObject, porque un MonoBehavior (Photo)
            // siempre es componente de un GameObject. 
            // Ademas, si Photo/Content deja de ser un MonoBehavior, no se puede asignar
            // a un GameObject en UnityEditor con referencias a otros elementos.
            GameObject auxGameObject =  new GameObject(filename);
            Photo photo = auxGameObject.AddComponent<Photo>();
            photo.thumbnail = thumbnail;
            photo.soundContainer = soundContainer;
            photo.nombre = filename;
            photo.imageMaterial = imageMaterial;
            
            // Tengo que agregar auxGameObject como hijo de ContentManager para que no se
            // elimine al cambiar de escena. Aparte acá no puedo usar DontDestroyOnLoad(), 
            // porque esto no es un MonoBehavior, y si lo vuelvo un MonoBehavior, no lo 
            // puedo crear con new en FileSystemContentLoader. Unity es un dolor de cabeza.
            Transform contentsTransform = cManager.transform.GetChild(1);
            Transform photosTransform = contentsTransform.GetChild(0);
            auxGameObject.transform.parent = photosTransform;
            return photo;
        }
    }
}
