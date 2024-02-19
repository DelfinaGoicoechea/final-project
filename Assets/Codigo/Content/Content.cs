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

        public abstract void play();

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
            AudioSource audioSource = ContentManager.Instance.audioSource;
            if (this.sound != null){
                audioSource.Pause();
            }
        }

        
    }
}