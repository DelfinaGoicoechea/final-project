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

        public override void LoadPlayAudio(Action assignmentAuxFunc = null)
        {
            AudioClip clip = null;
            // TODO: Probar cambiar a .mp3 y que funcione bien en Android
            UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(audioPath, AudioType.WAV);
            UnityWebRequestAsyncOperation uwrao = uwr.SendWebRequest();
            uwrao.completed += (result) => {
                // wrap tasks in try/catch, otherwise it'll fail silently
                try
                {
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
                    if (assignmentAuxFunc != null)
                        assignmentAuxFunc();
                }
                catch (Exception err)
                {
                    Debug.Log($"{err.Message}, {err.StackTrace}");
                }
                uwr.Dispose();
            };  
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