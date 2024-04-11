using UnityEngine;
using System;

namespace Contents{
    
    /// <summary>
    /// Clase encargada de representar al audio de un contenido, y controlar la
    /// carga y liberación de memoria asociada al archivo de audio.
    /// </summary>
    public class AudioContainer : MonoBehaviour
    {
        public AudioClip sound;
        
        [Tooltip("Nombre a mostrar a los usuarios")]
        public string nombre = "Default Audio Name";

        /// <summary>
        /// Carga y comienza la reproducción del audio.
        /// assignmentAuxFunc es util para la asignación del audio y
        /// comienzo de reproducción de forma asicrona.
        /// </summary>
        /// <param name="assignmentAuxFunc"></param>
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

        /// <summary>
        /// Libera la memoria asociada al audio.
        /// </summary>
        public virtual void UnloadAudio()
        {
            if (this.sound != null)
            {
                sound.UnloadAudioData();
            }
        }
    }
}