using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.Text.RegularExpressions;
using SimpleHttpServer.Models;

namespace Contents{
    public class FileSystemContentLoader
	{
        private ContentCreator contentCreator = new ContentCreator();
        private ContentManager cManager = GameObject.FindWithTag("ContentManager").GetComponent<ContentManager>();
        
        // TODO: dar soporte a .webp, usando https://github.com/netpyoung/unity.webp
        private Regex imageExtensionRegex = new Regex("(.jpg\\Z)|(.png\\Z)");
        private Regex audioExtensionRegex = new Regex("(.wav\\Z)|(.mp3\\Z)");

        // TODO: ver si se puede reproducir mkv
        private Regex videoExtensionRegex = new Regex("(.mp4\\Z)|(.webm\\Z)");
        private string imagesDirectory = Application.persistentDataPath;
        private string videosDirectory = Application.persistentDataPath;
        private string walksDirectory = Path.Combine(Application.persistentDataPath, "camino");
        private string audiosDirectory = Application.persistentDataPath;


        /// <summary>
        /// Encuentra un nombre de contenido valido para Fotos o Videos, y renombra
        /// al propio archivo que se intenta cargar desde el File System.
        /// </summary>
        /// <param name="contentFileInfo"></param>
        /// <param name="contents"></param>
        /// <param name="contentDirectory"></param>
        private void findValidContentFileName(FileInfo contentFileInfo, List<Content> contents, string contentDirectory){
            int i = 1;
            bool validNameFound = false;
            string filename = contentFileInfo.Name.Split('.')[0];
            string fileExt = contentFileInfo.Name.Split('.')[1];
            DirectoryInfo contentsDirectoryInfo = new DirectoryInfo(contentDirectory);
            FileInfo[] contentsFileInfos = contentsDirectoryInfo.GetFiles();
            char separator = Path.DirectorySeparatorChar;
            while (!validNameFound) {
                string newContentName = $"{filename} ({i})";
                // Busco Content.nombre repetido
                Content contentFound = contents.Find(x => x.nombre.Equals(newContentName));
                // Busco nombre de archivo repetido
                FileInfo fileFound = Array.Find(
                    contentsFileInfos,
                    finfo => finfo.Name.Split('.')[0].Equals(newContentName)
                );
                if (contentFound == null && fileFound == null) {
                    string newFilePath = $"{contentFileInfo.DirectoryName}{separator}{newContentName}.{fileExt}";
                    contentFileInfo.MoveTo(newFilePath);
                    validNameFound = true;
                } else {
                    i++ ;
                }
            }
        }

        /// <summary>
        /// Carga los audios que fueron enviados desde flutter antes de que se abriera Unity.
        /// </summary>
        /// <param name="audios"></param>
        /// <returns></returns>
        public List<AudioContainer> LoadAudios(List<AudioContainer> audios)
        {
            List<AudioContainer> audioList = new List<AudioContainer>();
            if (Directory.Exists(audiosDirectory))
            {
                string audiosDirectory = Application.persistentDataPath;
                DirectoryInfo audiosDirectoryInfo = new DirectoryInfo(audiosDirectory);
                Transform audiosTransform = cManager.transform.GetChild(1);
                foreach (var audioFileInfo in audiosDirectoryInfo.GetFiles())
                {
                    if(audioExtensionRegex.IsMatch(audioFileInfo.FullName))
                    {
                        AudioContainer audioContainer = LoadSingleAudio(audioFileInfo, audios);
                        audioList.Add(audioContainer);
                    }
                }
            } else {
                Directory.CreateDirectory(audiosDirectory);
            }
            return audioList;
        }

        /// <summary>
        /// Carga un unico audio ubicado en el path indicado en audioFileInfo.
        /// <br/>Recibe la lista de audios para poder saber si debe renombrar
        /// alguno de los audios debido a ser un nombre repetido.
        /// </summary>
        /// <param name="audioFileInfo"></param>
        /// <param name="audios"></param>
        /// <returns></returns>
        public AudioContainer LoadSingleAudio(FileInfo audioFileInfo, List<AudioContainer> audios){
            string filename = Path.GetFileNameWithoutExtension(audioFileInfo.FullName);
            string fileExt = audioFileInfo.Name.Split('.')[1];
            AudioContainer audioFound = audios.Find(x => x.nombre.Equals(filename));
            if(audioFound != null){
                int i = 1;
                bool validNameFound = false;
                DirectoryInfo audiosDirectoryInfo = new DirectoryInfo(audiosDirectory);
                FileInfo[] audiosFileInfos = audiosDirectoryInfo.GetFiles();
                string newAudioName = "";
                while (!validNameFound) {
                    newAudioName = $"{filename} ({i})";
                    // Busco AudioContainer.nombre repetido
                    audioFound = audios.Find(x => x.nombre.Equals(newAudioName));
                    // Busco nombre de archivo repetido (no tengo en cuenta la extension. Es intencional)
                    FileInfo fileFound = Array.Find(
                        audiosFileInfos,
                        finfo => finfo.Name.Split('.')[0].Equals(newAudioName)
                    );
                    if (audioFound == null && fileFound == null) {
                        string newFilePath = $"{audioFileInfo.DirectoryName}{Path.DirectorySeparatorChar}{newAudioName}.{fileExt}";
                        audioFileInfo.MoveTo(newFilePath);
                        validNameFound = true;
                    } else {
                        i++ ;
                    }
                }
                filename = newAudioName ;
            }
            Debug.Log($"Agrego el audio: {audioFileInfo.FullName}");
            GameObject auxGameObject =  new GameObject("audio "+filename);
            Transform audiosTransform = cManager.transform.GetChild(1);
            auxGameObject.transform.parent = audiosTransform;
            PathAudioContainer audioContainer = auxGameObject.AddComponent<PathAudioContainer>();
            audioContainer.nombre = filename;
            audioContainer.audioPath = audioFileInfo.FullName;
            return audioContainer;
        }


        /// <summary>
        /// Carga las imagenes ubicadas en el filesystem.
        /// <br/>Recibe la lista de contenidos para poder saber si debe renombrar
        /// alguno de los contenidos debido a ser un nombre repetido.
        /// </summary>
        /// <param name="contents"></param>
        /// <returns></returns>
        public List<PathPhoto> LoadImages(List<Content> contents)
        {
            List<PathPhoto> imageList = new List<PathPhoto>();
            if (Directory.Exists(Application.persistentDataPath))
            { 
                DirectoryInfo imagesDirectoryInfo = new DirectoryInfo(imagesDirectory);
                foreach (FileInfo imageFileInfo in imagesDirectoryInfo.GetFiles())
                {
                    if(imageExtensionRegex.IsMatch(imageFileInfo.FullName))
                    {
                        PathPhoto photo = LoadSingleImage(imageFileInfo, contents);
                        imageList.Add(photo);
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(imagesDirectory);
            }
            return imageList;
        }

        /// <summary>
		/// Este metodo sirve para que la imagen que fue pasada usando adb, ahora
		/// pueda ser cargada al ContentManager sin la necesidad de reiniciar la aplicacion.
        /// <br/>Recibe la lista de contenidos para poder saber si debe renombrar
        /// alguno de los contenidos debido a ser un nombre repetido.
		/// </summary>
		/// <param name="fileinfo"></param>
		/// <returns></returns>
		public PathPhoto LoadSingleImage(FileInfo imageFileInfo, List<Content> contents){
            string filename = imageFileInfo.Name.Split('.')[0];
            Content contentFound = contents.Find(x => x.nombre.Equals(filename));
            if (contentFound != null) {
                // Hay un content con este mismo nombre, asi que tengo que encontrar un nuevo nombre
                // valido y renombrar este archivo antes de crear el nuevo Content.
                findValidContentFileName(imageFileInfo, contents, imagesDirectory);
            }
            Debug.Log($"Agrego el contenido: {imageFileInfo.FullName}");
            PathPhoto photo = contentCreator.CreatePathPhoto(imageFileInfo);
            return photo;
        }


        /// <summary>
        /// Carga los videos ubicados en el filesystem.
        /// <br/>Recibe la lista de contenidos para poder saber si debe renombrar
        /// alguno de los contenidos debido a ser un nombre repetido.
        /// </summary>
        /// <param name="contents"></param>
        /// <returns></returns>
        public List<PathVideo> LoadVideos(List<Content> contents)
        {
            List<PathVideo> videoList = new List<PathVideo>();
            if (Directory.Exists(Application.persistentDataPath))
            { 
                DirectoryInfo videosDirectoryInfo = new DirectoryInfo(videosDirectory);
                foreach (var file in videosDirectoryInfo.GetFiles())
                {
                    if(videoExtensionRegex.IsMatch(file.FullName))
                    {
                        PathVideo video = LoadSingleVideo(file, contents);
                        videoList.Add(video);
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(videosDirectory);
            }
            return videoList;
        }

        /// <summary>
		/// Este metodo sirve para que el video que fue pasado usando adb, ahora
		/// pueda ser cargado al ContentManager sin la necesidad de reiniciar la aplicacion.
		/// </summary>
		/// <param name="fileinfo"></param>
		/// <returns></returns>
        public PathVideo LoadSingleVideo(FileInfo videoFileInfo, List<Content> contents){
            string filename = videoFileInfo.Name.Split('.')[0];
            Content contentFound = contents.Find(x => x.nombre.Equals(filename));
            if (contentFound != null) {
                // Hay un content con este mismo nombre, asi que tengo que encontrar un nuevo nombre
                // valido y renombrar este archivo antes de crear el nuevo Content.
                findValidContentFileName(videoFileInfo, contents, videosDirectory);
            }
            Debug.Log($"Agrego el contenido: {videoFileInfo.FullName}");
            PathVideo video = contentCreator.CreatePathVideo(videoFileInfo);
            return video;
		}


        /// <summary>
        /// Carga las externalApps indicadas en el archivo "externalApps.txt".
        /// <br/>Se pasa la lista de contenidos para saber si se debe renombrar
        /// alguna app en caso de ser un nombre repetido.
        /// </summary>
        /// <param name="contents"></param>
        /// <returns></returns>
        public List<ExternalApp> LoadExternalApps(List<Content> contents){
            List<ExternalApp> appsList = new List<ExternalApp>();
            if (Directory.Exists(Application.persistentDataPath)){ 
                string externalAppsFileLocation = $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}externalApps.txt";
                if(File.Exists(externalAppsFileLocation)){
                    string[] stringAppInfos = File.ReadAllLines(externalAppsFileLocation);
                    ContentCreator contentCreator = new ContentCreator();
                    foreach (string stringAppInfo in stringAppInfos){
                        ExternalAppInfo extAppInfo = ExternalAppInfo.CreateFromJSON(stringAppInfo);
                        string name = extAppInfo.visibleName;
                        string packg = extAppInfo.packageName;
                        // Deberia chequear el name antes de crearla.
                        // En caso de ser repetido deberia cambiar el nombre guardado
                        ExternalApp extApp = LoadSingleExternalApp(name, packg, contents);
                        appsList.Add(extApp);
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(Application.persistentDataPath);
            }
            return appsList;
        }


        /// <summary>
        /// Carga una unica app. Es util para cargar nuevas apps sin la necesidad
        /// de reiniciar Unity.
        /// <br/>Se pasa el listado de contenidos para saber si de debe renombrar
        /// alguna app por ser un nombre repetido.
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="packageName"></param>
        /// <param name="contents"></param>
        /// <returns></returns>
        public ExternalApp LoadSingleExternalApp(string appName,  string packageName, List<Content> contents){
            // La app ya fue agregada al listado de aplicaciones (externalApps.txt) desde
			// Flutter, por lo que no es necesario agregarle nada al listado. PERO, puede
            // que el nombre ya exista en el listado de contenidos, por lo que puedo
            // llegar a necesitar modificar el nombre utilizado dentro del listado.
            Content contentFound = contents.Find(x => x.nombre.Equals(appName));
            if(contentFound != null){
                // Encontre un contenido con el mismo nombre, toca renombrar
                string extAppsFileLocation = $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}externalApps.txt";
                string[] stringAppInfos = File.ReadAllLines(extAppsFileLocation);
                int i = 1;
                string newAppName = "";
                bool validNameFound = false;
                while (!validNameFound) {
                    newAppName = $"{appName} ({i})";
                    // Busco Content.nombre repetido
                    contentFound = contents.Find(x => x.nombre.Equals(newAppName));
                    if (contentFound == null) {
                        validNameFound = true;
                    } else {
                        i++ ;
                    }
                }
                // Escribo los cambios (borro el archivo para rellenarlo con info actualizada)
                StreamWriter streamWriter = File.CreateText(extAppsFileLocation);
                foreach (string stringAppInfo in stringAppInfos){
                    ExternalAppInfo extAppInfo = ExternalAppInfo.CreateFromJSON(stringAppInfo);
                    if  (appName == extAppInfo.visibleName) {
                        // escribo con nombre cambiado
                        extAppInfo.visibleName = newAppName;
                    }
                    streamWriter.WriteLine(extAppInfo.CreateJSON());
                }
                streamWriter.Close();
                appName = newAppName;
            }
            ExternalApp extApp = contentCreator.CreateExternalApp(appName, packageName);
            return extApp;
        }


        /// <summary>
        /// Carga los walks de Street View que fueron almacenados en el sistema de archivos.
        /// <br/><br/> Por ahora no se hace chequeo de nombre repetido.
        /// <br/>(PENDIENTE EN CASO DE TERMINAR IMPLEMENTACION)
        /// </summary>
        /// <returns></returns>
        public List<Walk> LoadWalks()
        {
            List<Walk> imageList = new List<Walk>();
            if (Directory.Exists(walksDirectory))
            { 
                string[] subDirectories = Directory.GetDirectories(walksDirectory);
                foreach (string subDirectory in subDirectories)
                {
                    // Obtiene el nombre de la subcarpeta
                    string directoryName = Path.GetFileName(subDirectory);
                    Walk walk = contentCreator.CreateWalk(directoryName);
                    string auxPath = Path.Combine(walksDirectory, directoryName);
                    DirectoryInfo directoryInfo = new DirectoryInfo(auxPath);
                    foreach (var file in directoryInfo.GetFiles())
                    {
                        if (imageExtensionRegex.IsMatch(file.FullName))
                        {
                            Debug.Log($"Agrego la imagen de camino: {file.FullName}");
                            Photo photo = contentCreator.CreatePhotoForWalk(file);
                            walk.addPhoto(photo);
                            byte[] bytes = File.ReadAllBytes(file.FullName);
                            Texture2D loadTexture = new Texture2D(1, 1, TextureFormat.RGB24, false);
                            ImageConversion.LoadImage(loadTexture, bytes);
                            Shader shader = Shader.Find("Skybox/Panoramic");
                            photo.imageMaterial = new Material(shader);
                            string filename = file.Name.Split('.')[0];
                            photo.thumbnail = ContentCreator.GetThumbnail(filename);
                            photo.imageMaterial.mainTexture = loadTexture;
                            photo.imageMaterial.renderQueue = 2450; // Hace que no se superpongan las imagenes de las esfera
                        }
                        walk.setThumbnail(walk.getFirst().getThumbnail());
                    }
                    imageList.Add(walk);
                }
            }
            else
            {
                Debug.LogWarning("La carpeta 'camino' no existia en persistentDataPath.");
                Directory.CreateDirectory(walksDirectory);
            }
        return imageList;
        }
    }
    
}