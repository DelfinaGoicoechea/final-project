using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Contents{

    
    public class Escena: Content
    {
       [Tooltip("Nombre de la escena a cargar (solo util y visible para Unity)")]
        public string sceneName;
        // Start is called before the first frame update
        void Start()
        {
            //or else
        }

        // Update is called once per frame
        void Update()
        {

        }
        
        public override void play()
        {
            SceneManager.LoadScene(sceneName);
            AudioSource audioSource = ContentManager.Instance.audioSource;
            audioSource.Pause();
            if (this.sound != null)
            {
                audioSource.clip = this.sound;
                audioSource.Play();
            }
        }

        public override string getContentType(){
            return "3D";
        }

        // Si aca ponemos un pause() que bloquee el movimiento del jugador,
        // tenemos que asegurarnos de rehabilitarlo al poner play en otra escena
    }
}