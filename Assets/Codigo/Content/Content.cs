using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Contents{
    
    /// <summary>
    /// Tipo abstracto de contenido, luego extendido por Fotos, Videos y Escenas 3D.
    /// </summary>
    public abstract class Content : MonoBehaviour
    {
        public Texture2D thumbnail;
        public AudioClip sound;
        
        [Tooltip("Nombre a mostrar a los usuarios")]
        public string nombre;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public virtual void play(){
            // Paro la reproduccion del sonido de cualquier Content previo
            AudioSource audioSource = ContentManager.Instance.audioSource;
            audioSource.Pause();

            if (this.sound != null)
            {
                if (sound.loadState == AudioDataLoadState.Unloaded)
                {
                    sound.LoadAudioData();
                }
                audioSource.clip = this.sound;
                audioSource.Play();
            }
        }

        /// <summary>
        /// Agrega el nombre del contenido a un Diccionario
        /// que contiene listas de nombres de contenidos
        /// </summary>
        /// <param name="dictio"></param>
        public abstract void addToDictionary(Dictionary<string, List<string>> dictio);

        /// <summary>
        /// Pauses any music plying in the scene
        /// </summary>
        public virtual void pause()
        {
            // TODO: audioSource se podria pausar desde pause() en contentManager,
            // estas lineas parecen ser innecesarias y sobrecomplicar el asunto.
            AudioSource audioSource = ContentManager.Instance.audioSource;
            if (this.sound != null){
                audioSource.Pause();
            }
        }

        public virtual void stop()
        {
            AudioSource audioSource = ContentManager.Instance.audioSource;
            if (this.sound != null){
                audioSource.Stop();
                sound.UnloadAudioData();
            }
        }
    }
}