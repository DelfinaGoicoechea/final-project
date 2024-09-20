using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.IO;
using System;

namespace Contents{

    /// <summary>
    /// Definicion del tipo de contenido video
    /// </summary>
    public class PathVideo : Video
    {
        public string videoFilePath;

        protected override IEnumerator waitForSceneLoadThenPlay(string sceneName, AudioSource audioSource)
        {
            // Busy wait, pero el delay es que debo esperar realmente es minimo
            while (SceneManager.GetActiveScene().name != sceneName)
            {
                yield return null;
            }

            if (SceneManager.GetActiveScene().name == sceneName)
            {
                GameObject sphere;
                ContentManager cm = ContentManager.Instance;
                sphere = cm.getSphere();

                // Asigno el material a la esfera
                MeshRenderer meshR = sphere.GetComponent<MeshRenderer>();
                Material[] meshmaterials = meshR.materials;
                meshmaterials[0] = material;

                VideoPlayer videoPlayer = GameObject.FindWithTag("VideoPlayer").GetComponent<VideoPlayer>();

                if (isPaused)
                {
                    videoPlayer.Play();
                    isPaused = false;
                    if (this.soundContainer != null){
                        audioSource.Play();
                    }
                    yield break;
                }
                
                // Asigno RenderTexture y video al VideoPlayer
                videoPlayer.Stop();
                videoPlayer.clip = null;
                videoPlayer.url = $"file://{videoFilePath}";

                if (this.soundContainer != null)
                {
                    try
                    {
                        videoPlayer.SetDirectAudioVolume(0, 0);
                        playAudioSource();
                    }
                    catch (Exception err)
                    {
                        Debug.Log($"{err.Message}, {err.StackTrace}");
                    }
                }
                else
                {
                    try
                    {
                        videoPlayer.SetDirectAudioVolume(0, 1);
                    }
                    catch (Exception err)
                    {
                        Debug.Log($"{err.Message}, {err.StackTrace}");
                    }
                }
                // Creo un rendertexture a partir de la resolucion de video, y le doy play
                videoPlayer.prepareCompleted += this.getVideoSizeAndPlay;
                videoPlayer.Prepare();
                // Acomodo la rotacion de la esfera porque el shader EAC esta rotado 90 grados
                sphere.transform.eulerAngles = (isEquiangularCubemap) ? eacRotation : noRotation;
                meshR.materials = meshmaterials;
            }
            isPaused = false;
            if(thumbnail == null || thumbnail == Texture2D.whiteTexture){
                StartCoroutine(captureThumbnail(sceneName));
            }
        }

        protected IEnumerator captureThumbnail(string sceneName){
            VideoPlayer videoPlayer = GameObject.FindWithTag("VideoPlayer").GetComponent<VideoPlayer>();
            
            while (SceneManager.GetActiveScene().name == sceneName && !(videoPlayer.isPlaying && videoPlayer.isPrepared))
            {
                yield return new WaitForSeconds(1);
            }

            // Espero 2 segundos mas para asegurarme por completo de que el primer frame ya haya cargado
            // (No encontre una manera confiable de esperar al primer frame cuando se usa RenderTexture)
            yield return new WaitForSeconds(2);

            // Me aseguro de que se siga en la misma escena, y no se haya cambiado de escena en esos 2 segundos
            if(SceneManager.GetActiveScene().name == sceneName){
                Texture2D image = ThumbnailCreator.Capture(this.nombre);
                this.thumbnail = image;
            }
        }

        public override bool delete () {
            // delete video
            try {
                File.Delete(videoFilePath);
            } catch (Exception e) {
                Debug.Log(e.StackTrace);
                return false;
            }
            // delete thumbnail
            if (thumbnail != null && thumbnail != Texture2D.whiteTexture) {
                UnityEngine.Object.Destroy(thumbnail);
                char OsSep = Path.DirectorySeparatorChar;
                string thumbLocation = $"{Application.persistentDataPath}{OsSep}thumbnails{OsSep}{nombre}.png";
                try {
                    File.Delete(thumbLocation);
                } catch (Exception e) {
                    Debug.Log(e.StackTrace);
                    return false;
                }
            }
            return base.delete();
        }

        public override bool changeNombre(string newName){
            char OsSep = Path.DirectorySeparatorChar;
            string oldThPath = $"{Application.persistentDataPath}{OsSep}thumbnails{OsSep}{nombre}.png";
            string newThPath = $"{Application.persistentDataPath}{OsSep}thumbnails{OsSep}{newName}.png";

            // Cambio el nombre del archivo de thumbnail en caso de que exista
            if (thumbnail != null && thumbnail != Texture2D.whiteTexture) {
                if (File.Exists(oldThPath)) {
                    try {
                        File.Move(oldThPath, newThPath);
                    } catch (Exception e) {
                        Debug.LogWarning(e.StackTrace);
                        return false;
                    }
                }
            } else {
                // Puede que no tenga thumbnail, pero solo porque se cambio el nombre del contenido
                // respecto del nombre original.
                // Entonces al buscar bajo nombre no se encontro un thumbnail, ya que aparece bajo el
                // nombre cambiado.
                // Esto surge de "ContentManager -> LoadCustomization()"
                if (File.Exists(newThPath)) {
                    // cargo la nueva textura.
                    byte[] bytes = File.ReadAllBytes(newThPath);
                    Texture2D loadTexture = new Texture2D(1, 1, TextureFormat.RGB24, false);
                    ImageConversion.LoadImage(loadTexture, bytes);
                    this.thumbnail = loadTexture;
                }
            }
            this.nombre = newName;
            return true;
        }
    }
}