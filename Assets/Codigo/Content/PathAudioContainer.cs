using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System;

namespace Contents{
    
    /// <summary>
    /// Tipo abstracto de contenido, luego extendido por Fotos, Videos y Escenas 3D.
    /// </summary>
    public class PathAudioContainer : AudioContainer
    {
        public string audioPath;

        public override void LoadAudio()
        {
            AudioClip clip = null;
            // TODO: Probar cambiar a .mp3 y que funcione bien en Android
            using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(audioPath, AudioType.WAV))
            {
                UnityWebRequestAsyncOperation uwrao = uwr.SendWebRequest();
                uwrao.completed += (result) => {
                    // wrap tasks in try/catch, otherwise it'll fail silently
                    try
                    {
                        //while (!uwr.isDone) await Task.Delay(5);
                        bool isNetworkError = uwr.result == UnityWebRequest.Result.ConnectionError;
                        bool isHttpError = uwr.result == UnityWebRequest.Result.ProtocolError;
                        if (isNetworkError || isHttpError) Debug.Log($"{uwr.error}");
                        else
                        {
                            clip = DownloadHandlerAudioClip.GetContent(uwr);
                        }
                        string filename = Path.GetFileNameWithoutExtension(audioPath);
                        clip.name = filename;
                        sound = clip;
                    }
                    catch (Exception err)
                    {
                        Debug.Log($"{err.Message}, {err.StackTrace}");
                    }
                };
            }
        }

        public override void UnloadAudio()
        {
            if (this.sound != null)
            {
                sound.UnloadAudioData();
            }
        }
    }
}