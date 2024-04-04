using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace Contents{
    public class FileSystemContentLoader
	{
        private ContentCreator contentCreator = new ContentCreator();

        public List<Photo> LoadImages()
        {
            List<Photo> imageList = new List<Photo>();
            if (Directory.Exists(Application.persistentDataPath))
            {
                
                string worldsFolder = Application.persistentDataPath;
        
                DirectoryInfo d = new DirectoryInfo(worldsFolder);
                // TODO: cargar tambien imagenes png
                foreach (var file in d.GetFiles("*.jpg"))
                {
                    Debug.Log(file.FullName);
                    Photo photo = contentCreator.CreatePhoto(file);
                    imageList.Add(photo);
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