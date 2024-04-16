using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System;

namespace Contents{
    
    /// <summary>
    /// Clase encargada de representar al audio proveniente sel sistema
    /// de archivos de un contenido, y controlar la carga y liberación
    /// de memoria asociada al archivo de audio.
    /// </summary>
    public class PathAudioContainer : AudioContainer
    {
        public string audioPath;

        /// <summary>
        /// Carga y comienza la reproducción del sonido.
        /// assignmentAuxFunc es util para la asignación del audio y
        /// comienzo de reproducción de forma asicrona.
        /// </summary>
        /// <param name="assignmentAuxFunc"></param>
        public override void LoadPlayAudio(Action assignmentAuxFunc = null)
        {
            AudioClip clip = null;
            // TODO: Probar cambiar a .mp3 y que funcione bien en Android
            UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip("file://"+audioPath, AudioType.WAV);
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

        /// <summary>
        /// Libera la memoria asociada al audio.
        /// </summary>
        public override void UnloadAudio()
        {
            if (this.sound != null)
            {
                sound.UnloadAudioData();
            }
        }
    }
}