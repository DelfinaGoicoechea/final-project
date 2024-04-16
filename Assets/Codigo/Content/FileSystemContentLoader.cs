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
                // TODO: Averiguar porque no reproduce .mp3 en Android
                Regex regex = new Regex("(.wav\\Z)");
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
    }
}