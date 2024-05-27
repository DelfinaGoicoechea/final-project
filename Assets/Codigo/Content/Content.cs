using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Contents{
    
    /// <summary>
    /// Tipo abstracto de contenido, luego extendido por Fotos, Videos y Escenas 3D.
    /// </summary>
    public abstract class Content : MonoBehaviour
    {
        public Texture2D thumbnail;
        public AudioContainer soundContainer;
        
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

            if (this.soundContainer != null)
            {
                // Es necesario pasar una funcion que asigne y comience la reproduccion (assignmentAuxFunc),
                // porque soundContainer.LoadPlayAudio() puede ejecutar la obtencion del
                // sonido de forma asincrona (PathAudioContainer), por lo que no está asegurado
                // que el audio esté cargado para darle play luego de LoadPlayAudio().
                Action assignmentAuxFunc = () => {
                    audioSource.clip = this.soundContainer.sound;
                    audioSource.Play();
                };
                soundContainer.LoadPlayAudio(assignmentAuxFunc);
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
            if (this.soundContainer != null){
                audioSource.Pause();
            }
        }

        public virtual void stop()
        {
            AudioSource audioSource = ContentManager.Instance.audioSource;
            if (this.soundContainer != null){
                audioSource.Stop();
                soundContainer.UnloadAudio();
            }
        }

        public virtual Dictionary<string,string> getCurrentConfig(){
            Dictionary<string,string> config = new Dictionary<string,string>();
            config["name"] = this.nombre;
            if (soundContainer != null){
                config["sound"] = soundContainer.nombre;
            }
            return config;
        }

        public abstract bool delete ();
    }
}