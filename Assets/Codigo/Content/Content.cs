using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Contents{
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
        public abstract string getContentType();

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