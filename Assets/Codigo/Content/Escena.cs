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
        private bool isPaused = false;
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

            // Solo es necesario rehabilitar el control de movimiento
            // si el estado actual de la escena es pausado.
            if(isPaused){
                GameObject player = GameObject.FindWithTag("Player");
                if(player == null)
                {
                    Debug.LogWarning("No se encuentra el controlador del jugador (GameObject con tag Player)");
                }
                else
                {
                    // TODO: Reactivar movimiento
                    /*
                    Transform xrOrigin = player.transform.GetChild(0);
                    GameObject locomotionSystem = xrOrigin.GetChild(1).gameObject;
                    locomotionSystem.SetActive(true);
                    */
                }
            }
        }

        public override void pause()
        {
            base.pause();
            isPaused = true;
            StartCoroutine(ensureSceneLoadThenPause());

        }
        
        /*
        public override void stop()
        {
            base.stop();
            isPaused = false;
        }
        */

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

        /// <summary>
        /// Se asegura de que ya se cargó la escena,
        /// y luego deshabilita el control de movimiento 
        /// </summary>
        /// <returns></returns>
        private IEnumerator ensureSceneLoadThenPause()
        {
            // Busy wait, pero el delay que debo esperar realmente es minimo
            while (SceneManager.GetActiveScene().name != sceneName)
            {
                yield return null;
            }
            
            GameObject player = GameObject.FindWithTag("Player");
            if(player == null)
            {
                Debug.LogWarning("No se encuentra el controlador del jugador (GameObject con tag Player)");
            }
            else
            {
                // TODO: Desactivar movimiento
                /*
                Transform xrOrigin = player.transform.GetChild(0);
                GameObject locomotionSystem = xrOrigin.GetChild(1).gameObject;
                locomotionSystem.SetActive(false);
                */
            }
        }
    }
}