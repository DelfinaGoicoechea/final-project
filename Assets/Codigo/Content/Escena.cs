using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Contents{
    public class Escena: Content
    {
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
            SceneManager.LoadScene(nombre);
            AudioSource audioSource = ContentManager.Instance.audioSource;
            audioSource.Pause();
            if (this.sound != null)
            {
                audioSource.clip = this.sound;
                audioSource.Play();
            }
        }

        // Si aca ponemos un pause() que bloquee el movimiento del jugador,
        // tenemos que asegurarnos de rehabilitarlo al poner play en otra escena
    }
}