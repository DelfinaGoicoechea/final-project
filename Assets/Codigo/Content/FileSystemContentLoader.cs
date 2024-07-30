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


        public List<AudioContainer> LoadAudios()
        {
            List<AudioContainer> audioList = new List<AudioContainer>();
            if (Directory.Exists(Application.persistentDataPath))
            {
                string audiosDirectory = Application.persistentDataPath;
                DirectoryInfo audiosDirectoryInfo = new DirectoryInfo(audiosDirectory);
                Transform audiosTransform = cManager.transform.GetChild(2);
                foreach (var file in audiosDirectoryInfo.GetFiles())
                {
                    if(audioExtensionRegex.IsMatch(file.FullName))
                    {
                        Debug.Log($"Agrego el audio: {file.FullName}");
                        string filename = Path.GetFileNameWithoutExtension(file.FullName);
                        GameObject auxGameObject =  new GameObject("audio "+filename);
                        auxGameObject.transform.parent = audiosTransform;
                        PathAudioContainer audioContainer = auxGameObject.AddComponent<PathAudioContainer>();
                        audioContainer.nombre = filename;
                        audioContainer.audioPath = file.FullName;
                        audioList.Add(audioContainer);
                    }
                }
            }
            return audioList;
        }

        public AudioContainer LoadSingleAudio(FileInfo fileinfo){
            Debug.Log($"Agrego el audio: {fileinfo.FullName}");
            string filename = Path.GetFileNameWithoutExtension(fileinfo.FullName);
            GameObject auxGameObject =  new GameObject("audio "+filename);
            Transform audiosTransform = cManager.transform.GetChild(2);
            auxGameObject.transform.parent = audiosTransform;
            PathAudioContainer audioContainer = auxGameObject.AddComponent<PathAudioContainer>();
            audioContainer.nombre = filename;
            audioContainer.audioPath = fileinfo.FullName;
            return audioContainer;
        }

        public List<PathPhoto> LoadImages()
        {
            List<PathPhoto> imageList = new List<PathPhoto>();
            if (Directory.Exists(Application.persistentDataPath))
            { 
                string imagesDirectory = Application.persistentDataPath;
                DirectoryInfo imagesDirectoryInfo = new DirectoryInfo(imagesDirectory);
                foreach (var file in imagesDirectoryInfo.GetFiles())
                {
                    if(imageExtensionRegex.IsMatch(file.FullName))
                    {
                        Debug.Log($"Agrego el contenido: {file.FullName}");
                        PathPhoto photo = contentCreator.CreatePathPhoto(file);
                        imageList.Add(photo);
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(Application.persistentDataPath);
            }
            return imageList;
        }

        /// <summary>
		/// Este metodo sirve para que la imagen que fue pasada usando adb, ahora
		/// pueda ser cargada al ContentManager sin la necesidad de reiniciar la aplicacion.
		/// </summary>
		/// <param name="fileinfo"></param>
		/// <returns></returns>
		public PathPhoto LoadSingleImage(FileInfo fileinfo){
            if(imageExtensionRegex.IsMatch(fileinfo.FullName)){
                Debug.Log($"Agrego el contenido: {fileinfo.FullName}");
                PathPhoto photo = contentCreator.CreatePathPhoto(fileinfo);
                return photo;
            }
            return null;
		}

        public List<PathVideo> LoadVideos()
        {
            List<PathVideo> videoList = new List<PathVideo>();
            if (Directory.Exists(Application.persistentDataPath))
            { 
                string imagesDirectory = Application.persistentDataPath;
                DirectoryInfo imagesDirectoryInfo = new DirectoryInfo(imagesDirectory);
                foreach (var file in imagesDirectoryInfo.GetFiles())
                {
                    if(videoExtensionRegex.IsMatch(file.FullName))
                    {
                        Debug.Log($"Agrego el contenido: {file.FullName}");
                        PathVideo video = contentCreator.CreatePathVideo(file);
                        videoList.Add(video);
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(Application.persistentDataPath);
            }
            return videoList;
        }

        /// <summary>
		/// Este metodo sirve para que el video que fue pasado usando adb, ahora
		/// pueda ser cargado al ContentManager sin la necesidad de reiniciar la aplicacion.
		/// </summary>
		/// <param name="fileinfo"></param>
		/// <returns></returns>
        public PathVideo LoadSingleVideo(FileInfo fileinfo){
            if(videoExtensionRegex.IsMatch(fileinfo.FullName)){
                Debug.Log($"Agrego el contenido: {fileinfo.FullName}");
                PathVideo video = contentCreator.CreatePathVideo(fileinfo);
                return video;
            }
            return null;
		}

        public List<ExternalApp> LoadExternalApps(){
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
                        ExternalApp extApp = contentCreator.CreateExternalApp(name, packg);
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

        public List<Walk> LoadWalks()
        {
            List<Walk> imageList = new List<Walk>();
            string path = Path.Combine(Application.persistentDataPath, "camino");
            if (Directory.Exists(path))
            { 
                string[] subDirectories = Directory.GetDirectories(path);
                foreach (string subDirectory in subDirectories)
                {
                    // Obtiene el nombre de la subcarpeta
                    string directoryName = Path.GetFileName(subDirectory);
                    Walk walk = contentCreator.CreateWalk(directoryName);
                    string auxPath = Path.Combine(path, directoryName);
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
            Debug.LogWarning("La carpeta 'camino' no existe en persistentDataPath.");
            }
        return imageList;
        }
    }
    
}