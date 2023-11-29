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
        if (SceneManager.GetActiveScene().name == "SphereScene")
        {
            VideoPlayer videoPlayer = GameObject.Find("VideoPlayer").GetComponent<VideoPlayer>();
            videoPlayer.Pause();
            AudioSource audioSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();
            audioSource.Pause();
            GameObject sphere = ContentManager.Instance.getSphere();
            StartCoroutine(checkRotation(sphere));
            MeshRenderer meshR = sphere.GetComponent<MeshRenderer>();
            Material[] meshmaterials = meshR.materials;
            meshmaterials[0] = material;
            meshR.materials = meshmaterials;
            if(this.sound != null)
            {
                audioSource.clip = this.sound;
                audioSource.Play();
            }

        }
        else
        {
            //TODO: Implementar despues de programar el cambio de escena
            //Espero a cambiar de Escena, probablemente antes pongo la de carga
            //Asigno la textura
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