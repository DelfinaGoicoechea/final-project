using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

namespace Contents{
    public class Photo : Content
    {
        public Material material;

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
            AudioSource audioSource = ContentManager.Instance.audioSource;
            audioSource.Pause();

            if (SceneManager.GetActiveScene().name == "SphereScene")
            {
                VideoPlayer videoPlayer = GameObject.FindWithTag("VideoPlayer").GetComponent<VideoPlayer>();
                videoPlayer.Pause();

                GameObject sphere = ContentManager.Instance.getSphere();
                
                MeshRenderer meshR = sphere.GetComponent<MeshRenderer>();
                Material[] meshmaterials = meshR.materials;
                meshmaterials[0] = material;
                meshR.materials = meshmaterials;

                GameObject player = GameObject.FindWithTag("Player");
                player.transform.position = sphere.transform.position;

                if (this.sound != null)
                {
                    audioSource.clip = this.sound;
                    audioSource.Play();
                }
            }
            else
            {
                SceneManager.LoadScene("SphereScene");
                if (this.sound != null)
                {
                    audioSource.clip = this.sound;
                    audioSource.Play();
                }

                // Espero a que carge la escena, para no tener problemas al buscar la esfera
                if (SceneManager.GetActiveScene().name != "SphereScene")
                {
                    StartCoroutine(waitForSceneLoad("SphereScene"));
                }
            }
        }

        private IEnumerator waitForSceneLoad(string sceneName)
        {
            // Busy wait, pero el delay que debo esperar realmente es m�nimo
            while (SceneManager.GetActiveScene().name != sceneName)
            {
                yield return null;
            }

            if(SceneManager.GetActiveScene().name == sceneName)
            {
                GameObject sphere = ContentManager.Instance.getSphere();
                MeshRenderer meshR = sphere.GetComponent<MeshRenderer>();
                Material[] meshmaterials = meshR.materials;
                meshmaterials[0] = material;
                meshR.materials = meshmaterials;
                GameObject player = GameObject.FindWithTag("Player");
                player.transform.position = sphere.transform.position;
            }
        }
    }
}