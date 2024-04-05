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

        public List<Photo> LoadImages()
        {
            List<Photo> imageList = new List<Photo>();
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
                        Photo photo = contentCreator.CreatePhoto(file);
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