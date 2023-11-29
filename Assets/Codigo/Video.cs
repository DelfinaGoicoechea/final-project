using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class Video : Content
{
    public VideoClip video;
    public Material material;
    public RenderTexture renderTexture;
    public bool equiangularCubemap;
    private bool isPaused;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void play()
    {
        // TODO: Asignar sonido al videoPlayer, o a un AudioSource
        VideoPlayer videoPlayer = GameObject.Find("VideoPlayer").GetComponent<VideoPlayer>();
        if (isPaused)
        {
            videoPlayer.Play();
            isPaused = false;
            return;
        }

        isPaused = false;
        GameObject sphere = ContentManager.Instance.getSphere();
        if (SceneManager.GetActiveScene().name == "SphereScene")
        {
            // Roto la esfera dependiendo de si es o no un Equi-Angular Cubemap
            StartCoroutine(checkRotation(sphere));

            // Asigno el material a la esfera
            MeshRenderer meshR = sphere.GetComponent<MeshRenderer>();
            Material[] meshmaterials = meshR.materials;
            meshmaterials[0] = material;
            meshR.materials = meshmaterials;

            // Asigno RenderTexture y video al VideoPlayer
            videoPlayer.clip = video;
            videoPlayer.targetTexture = renderTexture;
        }
        else
        {
            //TODO: Implementar despues de programar el cambio de escena
            //Espero a cambiar de Escena, probablemente antes pongo la de carga
            //Asigno la textura
        }
    }

    public override void pause()
    {
        VideoPlayer videoPlayer = GameObject.Find("VideoPlayer").GetComponent<VideoPlayer>();
        videoPlayer.Pause();
        isPaused = true;
    }

    private IEnumerator checkRotation(GameObject sphere)
    {
        // obtengo la posicion y rotacion local de la esfera
        Vector3 position = new Vector3(0.0f, 1.0f, 0.0f);
        Quaternion rotation = new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);
        sphere.transform.GetLocalPositionAndRotation(out position, out rotation);

        // Roto la esfera si es un Equi-Angular Cubemap (Porque no se cómo rotarlo desde el shader)
        Quaternion rotated = new Quaternion(0.00000f, 0.00000f, 0.70711f, 0.70711f);
        if (equiangularCubemap && rotation != rotated)
        {
            sphere.transform.Rotate(0.0f, 0.0f, 90.0f);
        }
        else
        {
            //Acomodo la esfera si ya estaba rotada y este video NO es un Equi-Angular Cubemap
            if (!equiangularCubemap && rotation == rotated)
            {
                sphere.transform.Rotate(0.0f, 0.0f, -90.0f);
            }
        }
        yield return null;
    }
}