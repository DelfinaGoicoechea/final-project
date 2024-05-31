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

        private static Texture2D GetThumbnail(string filename){
            
            string thumbFolder = $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}thumbnails";
            Directory.CreateDirectory(thumbFolder); // Confirma existencia de la carpeta de thumbnails
            string thumbFileLocation = $"{thumbFolder}{Path.DirectorySeparatorChar}{filename}.png";
            if (File.Exists(thumbFileLocation)){ // Confirma existencia del archivo
                byte[] bytes = File.ReadAllBytes(thumbFileLocation);
                Texture2D loadTexture = new Texture2D(1, 1, TextureFormat.RGB24, false);
                ImageConversion.LoadImage(loadTexture, bytes);
                //loadTexture.Compress(false);
                return loadTexture;
            }else{
                return Texture2D.whiteTexture;
            }
        }

        /// <summary>
	    /// Crea un contenido Photo a partir de el path a una imagen.
        /// La imagen debe ser de tipo jpg o png (al menos de momento).
        /// El GameObject asociado a la Photo, DEBE ser agregado a ContentManager,
        /// o se va a perder al cambiar de escena o ejecutar Stop.
	    /// </summary>
        public PathPhoto CreatePathPhoto(FileInfo imageFileInfo, AudioContainer soundContainer = null){
            // --------------- Crear Photo ---------------
            string filename = imageFileInfo.Name.Split('.')[0];
            Texture2D thumbnail = GetThumbnail(filename);

            // Se DEBE crear un GameObject, porque un MonoBehavior (Photo)
            // siempre es componente de un GameObject. 
            // Ademas, si Photo/Content deja de ser un MonoBehavior, no se puede asignar
            // a un GameObject en UnityEditor con referencias a otros elementos.
            GameObject auxGameObject =  new GameObject(filename);
            PathPhoto photo = auxGameObject.AddComponent<PathPhoto>();
            photo.thumbnail = thumbnail;
            photo.soundContainer = soundContainer;
            photo.nombre = filename;
            photo.imagePath = imageFileInfo.FullName;
            
            // Tengo que agregar auxGameObject como hijo de ContentManager para que no se
            // elimine al cambiar de escena. Aparte acá no puedo usar DontDestroyOnLoad(), 
            // porque esto no es un MonoBehavior, y si lo vuelvo un MonoBehavior, no lo 
            // puedo crear con new en FileSystemContentLoader. Unity es un dolor de cabeza.
            Transform contentsTransform = cManager.transform.GetChild(1);
            Transform photosTransform = contentsTransform.GetChild(0);
            auxGameObject.transform.parent = photosTransform;
            return photo;
        }
        
        /// <summary>
	    /// Crea un contenido PathVideo a partir de el path a un video.
        /// El video debe ser con extension mp4 o webm (al menos de momento).
        /// El GameObject asociado al PathVideo, DEBE ser agregado a ContentManager,
        /// o se va a perder al cambiar de escena o ejecutar Stop.
	    /// </summary>
        public PathVideo CreatePathVideo(FileInfo videoFileInfo, AudioContainer soundContainer = null){
            // --------------- Crear Video ---------------
            string filename = videoFileInfo.Name.Split('.')[0];
            Texture2D thumbnail = GetThumbnail(filename);
            

            // Se DEBE crear un GameObject, porque un MonoBehavior (Video)
            // siempre es componente de un GameObject. 
            // Ademas, si Video/Content deja de ser un MonoBehavior, no se puede asignar
            // a un GameObject en UnityEditor con referencias a otros elementos.
            GameObject auxGameObject =  new GameObject(filename);
            PathVideo video = auxGameObject.AddComponent<PathVideo>();
            video.thumbnail = thumbnail;
            video.soundContainer = soundContainer;
            video.nombre = filename;
            video.videoFilePath = videoFileInfo.FullName;

            // TODO: Intentar averiguar la proyeccion analizando metadata
            bool isEAC = true;
            video.isEquiangularCubemap = isEAC;
            if (isEAC){
                Shader shader = Shader.Find("Custom/EAC-sphere");
                Material renderMaterial = new Material(shader);
                renderMaterial.renderQueue = 2450; // Hace que no se superpongan las imagenes de las esferas
                video.material = renderMaterial;
            }else{
                Shader shader = Shader.Find("Sprites/Default");
                Material renderMaterial = new Material(shader);
                renderMaterial.renderQueue = 2450; // Hace que no se superpongan las imagenes de las esferas
                video.material = renderMaterial;
            }
            

            // Tengo que agregar auxGameObject como hijo de ContentManager para que no se
            // elimine al cambiar de escena. Aparte acá no puedo usar DontDestroyOnLoad(), 
            // porque esto no es un MonoBehavior, y si lo vuelvo un MonoBehavior, no lo 
            // puedo crear con new en FileSystemContentLoader. Unity es un dolor de cabeza.
            Transform contentsTransform = cManager.transform.GetChild(1);
            Transform videosTransform = contentsTransform.GetChild(1);
            auxGameObject.transform.parent = videosTransform;
            return video;
        }
    }
}
