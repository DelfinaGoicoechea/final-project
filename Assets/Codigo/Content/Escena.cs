using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Contents{

    /// <summary>
    /// Definicion del tipo de contenido escena 3D
    /// </summary>
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

        /// <summary>
        /// Agrega el nombre del contenido a un Diccionario que
        ///  contiene listas de nombres de contenidos, bajo la key
        ///  "3D".
        /// </summary>
        /// <param name="dictio"></param>
        public override void addToDictionary(Dictionary<string, List<string>> dictio){
            if (!dictio.ContainsKey("3D")){
                dictio.Add("3D", new List<string>());
            }
            dictio["3D"].Add(this.nombre);
        }

        // Si aca ponemos un pause() que bloquee el movimiento del jugador,
        // tenemos que asegurarnos de rehabilitarlo al poner play en otra escena
    }
}