using System.IO;
using UnityEngine;

namespace Contents{
	/// <summary>
	/// Clase encargada de gestionar la creacion de nuevos contenidos.
	/// </summary>
    public class ContentCreator
    {
        private ContentManager cManager = GameObject.FindWithTag("ContentManager").GetComponent<ContentManager>();

        public static Texture2D GetThumbnail(string filename){
            
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
            photo.customizationsEntry = "Execution";
            
            // Tengo que agregar auxGameObject como hijo de ContentManager para que no se
            // elimine al cambiar de escena. Aparte acá no puedo usar DontDestroyOnLoad(), 
            // porque esto no es un MonoBehavior, y si lo vuelvo un MonoBehavior, no lo 
            // puedo crear con new en FileSystemContentLoader. Unity es un dolor de cabeza.
            Transform contentsTransform = cManager.transform.GetChild(0);
            Transform photosTransform = contentsTransform.GetChild(0);
            auxGameObject.transform.parent = photosTransform;
            return photo;
        }


        public Walk CreateWalk(string name, AudioContainer soundContainer = null){
            GameObject auxGameObject = new GameObject(name);
            Walk camino = auxGameObject.AddComponent<Walk>(); 
            camino.nombre = name;
            camino.customizationsEntry = "Execution";
            Transform contentTransform = cManager.transform.GetChild(0);
            Transform walkTransform = contentTransform.GetChild(3);
            auxGameObject.transform.parent = walkTransform;
            return camino;
        }


        /// <summary>
	    /// Crea un contenido camino a partir de el path a una imagen.
        /// Las imagenes deben ser de tipo jpg o png (al menos de momento).
        /// El GameObject asociado a la Photo, DEBE ser agregado a ContentManager,
        /// o se va a perder al cambiar de escena o ejecutar Stop.
	    /// </summary>
        public Photo CreatePhotoForWalk(FileInfo imageFileInfo, AudioContainer soundContainer = null){
            string filename = imageFileInfo.Name.Split('.')[0];
            Texture2D thumbnail = GetThumbnail(filename);

            GameObject auxGameObject =  new GameObject(filename);
            Photo photo = auxGameObject.AddComponent<Photo>();
            photo.thumbnail = thumbnail;
            photo.soundContainer = soundContainer;
            photo.nombre = filename;
            photo.customizationsEntry = "Execution";
            
            Transform contentTransform = cManager.transform.GetChild(0);
            Transform walkTransform = contentTransform.GetChild(3);
            Transform photoTransform = walkTransform.GetChild(0);
            auxGameObject.transform.parent = photoTransform;
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
            video.customizationsEntry = "Execution";

            bool isEAC = true;
            video.isEquiangularCubemap = isEAC;
            if (isEAC){
                video.material = (Material)Resources.Load("360_EAC", typeof(Material));
            }else{
                video.material = (Material)Resources.Load("360_Material", typeof(Material));
            }
            

            // Tengo que agregar auxGameObject como hijo de ContentManager para que no se
            // elimine al cambiar de escena. Aparte acá no puedo usar DontDestroyOnLoad(), 
            // porque esto no es un MonoBehavior, y si lo vuelvo un MonoBehavior, no lo 
            // puedo crear con new en FileSystemContentLoader. Unity es un dolor de cabeza.
            Transform contentsTransform = cManager.transform.GetChild(0);
            Transform videosTransform = contentsTransform.GetChild(1);
            auxGameObject.transform.parent = videosTransform;
            return video;
        }

        /// <summary>
	    /// Crea un contenido ExternalApp a partir del nombre de paquete.
        /// El GameObject asociado a la ExternalApp, DEBE ser agregado a ContentManager,
        /// o se va a perder al cambiar de escena o ejecutar Stop.
	    /// </summary>
        public ExternalApp CreateExternalApp(string visibleName, string packageName){
            // --------------- Crear Photo ---------------
            Texture2D thumbnail = GetThumbnail(visibleName);

            // Se DEBE crear un GameObject, porque un MonoBehavior (Photo)
            // siempre es componente de un GameObject. 
            // Ademas, si Photo/Content deja de ser un MonoBehavior, no se puede asignar
            // a un GameObject en UnityEditor con referencias a otros elementos.
            GameObject auxGameObject =  new GameObject(visibleName);
            ExternalApp externalApp = auxGameObject.AddComponent<ExternalApp>();
            externalApp.thumbnail = thumbnail;
            externalApp.soundContainer = null;
            externalApp.nombre = visibleName;
            externalApp.packageName = packageName;
            externalApp.customizationsEntry = "Execution";
            
            // Tengo que agregar auxGameObject como hijo de ContentManager para que no se
            // elimine al cambiar de escena. Aparte acá no puedo usar DontDestroyOnLoad(), 
            // porque esto no es un MonoBehavior, y si lo vuelvo un MonoBehavior, no lo 
            // puedo crear con new en FileSystemContentLoader. Unity es un dolor de cabeza.
            Transform contentsTransform = cManager.transform.GetChild(0);
            Transform appsTransform = contentsTransform.GetChild(4);
            auxGameObject.transform.parent = appsTransform;
            
            return externalApp;
        }
    }
}
