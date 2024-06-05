using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.Text.RegularExpressions;

namespace Contents{
    public class FileSystemContentLoader
	{
        private ContentCreator contentCreator = new ContentCreator();
        private ContentManager cManager = GameObject.FindWithTag("ContentManager").GetComponent<ContentManager>();

        public List<AudioContainer> LoadAudios()
        {
            List<AudioContainer> audioList = new List<AudioContainer>();
            if (Directory.Exists(Application.persistentDataPath))
            {
                string audiosDirectory = Application.persistentDataPath;
                DirectoryInfo audiosDirectoryInfo = new DirectoryInfo(audiosDirectory);
                Regex regex = new Regex("(.wav\\Z)|(.mp3\\Z)");
                Transform audiosTransform = cManager.transform.GetChild(2);
                foreach (var file in audiosDirectoryInfo.GetFiles())
                {
                    if(regex.IsMatch(file.FullName))
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

        public List<PathPhoto> LoadImages()
        {
            List<PathPhoto> imageList = new List<PathPhoto>();
            if (Directory.Exists(Application.persistentDataPath))
            { 
                string imagesDirectory = Application.persistentDataPath;
                DirectoryInfo imagesDirectoryInfo = new DirectoryInfo(imagesDirectory);
                // TODO: dar soporte a .webp, usando https://github.com/netpyoung/unity.webp
                Regex regex = new Regex("(.jpg\\Z)|(.png\\Z)");
                foreach (var file in imagesDirectoryInfo.GetFiles())
                {
                    if(regex.IsMatch(file.FullName))
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

        public List<PathVideo> LoadVideos()
        {
            List<PathVideo> videoList = new List<PathVideo>();
            if (Directory.Exists(Application.persistentDataPath))
            { 
                string imagesDirectory = Application.persistentDataPath;
                DirectoryInfo imagesDirectoryInfo = new DirectoryInfo(imagesDirectory);
                // TODO: ver si se puede reproducir mkv
                Regex regex = new Regex("(.mp4\\Z)|(.webm\\Z)");
                foreach (var file in imagesDirectoryInfo.GetFiles())
                {
                    if(regex.IsMatch(file.FullName))
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
                    Regex regex = new Regex("(.jpg\\Z)|(.png\\Z)");
                    foreach (var file in directoryInfo.GetFiles())
                    {
                        if (regex.IsMatch(file.FullName))
                        {
                            Debug.Log($"Agrego la imagen de camino: {file.FullName}");
                            Photo photo = contentCreator.CreatePhotoForWalk(file);
                            walk.addPhoto(photo);
                            byte[] bytes = File.ReadAllBytes(file.FullName);
                            Texture2D loadTexture = new Texture2D(1, 1, TextureFormat.RGB24, false);
                            ImageConversion.LoadImage(loadTexture, bytes);
                            Shader shader = Shader.Find("Skybox/Panoramic");
                            photo.imageMaterial = new Material(shader);
                            photo.thumbnail = loadTexture;
                            photo.imageMaterial.mainTexture = loadTexture;
                            photo.imageMaterial.renderQueue = 2450; // Hace que no se superpongan las imagenes de las esfera
                        }
                        walk.setThumbnail(walk.getFirst().getThumbnail());
                    }
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