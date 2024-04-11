using UnityEngine;
using System;

namespace Contents{
    
    /// <summary>
    /// Tipo abstracto de contenido, luego extendido por Fotos, Videos y Escenas 3D.
    /// </summary>
    public class AudioContainer : MonoBehaviour
    {
        public AudioClip sound;
        
        [Tooltip("Nombre a mostrar a los usuarios")]
        public string nombre = "Default Audio Name";

        public virtual void LoadPlayAudio(Action assignmentAuxFunc = null)
        {
            if (this.sound != null)
            {
                if (sound.loadState == AudioDataLoadState.Unloaded)
                {
                    sound.LoadAudioData();
                    if(assignmentAuxFunc != null)
                        assignmentAuxFunc();
                }
            }
        }

        public virtual void UnloadAudio()
        {
            if (this.sound != null)
            {
                sound.UnloadAudioData();
            }
        }
    }
}