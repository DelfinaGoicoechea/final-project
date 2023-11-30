using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

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
            StartCoroutine(checkRotation(sphere));
            MeshRenderer meshR = sphere.GetComponent<MeshRenderer>();
            Material[] meshmaterials = meshR.materials;
            meshmaterials[0] = material;
            meshR.materials = meshmaterials;

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
        // Busy wait, pero el delay es que debo esperar realmente es mínimo
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
        }
    }

    private IEnumerator checkRotation(GameObject sphere)
    {
        // obtengo la posicion y rotacion local de la esfera
        Vector3 position = new Vector3(0.0f, 1.0f, 0.0f);
        Quaternion rotation = new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);
        sphere.transform.GetLocalPositionAndRotation(out position, out rotation);

        //Acomodo la esfera si estaba rotada
        Quaternion rotated = new Quaternion(0.00000f, 0.00000f, 0.70711f, 0.70711f);
        if (rotation == rotated)
        {
            sphere.transform.Rotate(0.0f, 0.0f, -90.0f);
        }
        yield return null;
    }
}