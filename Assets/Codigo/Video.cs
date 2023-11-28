using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class Video : Content
{
    //public VideoPlayer videoPlayer;
    public VideoClip video;
    public Material material;
    public RenderTexture renderTexture;
    public bool equiangularCubemap;
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
            GameObject sphere = ContentManager.Instance.getSphere();

            // obtengo la rotacion local de la esfera para usarla despues
            Vector3 vec = new Vector3(0.0f, 1.0f, 0.0f);
            Quaternion qu = new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);
            sphere.transform.GetLocalPositionAndRotation(out vec, out qu);

            // Asigno el material a la esfera
            MeshRenderer meshR = sphere.GetComponent<MeshRenderer>();
            Material[] meshmaterials = meshR.materials;
            meshmaterials[0] = material;
            meshR.materials = meshmaterials;

            // Roto la esfera si es un Equi-Angular Cubemap
            // (Porque no se cómo rotarlo desde el shader)
            Quaternion rotated = new Quaternion(0.00000f, 0.00000f, 0.70711f, 0.70711f);
            if (equiangularCubemap && qu != rotated)
            {
                sphere.transform.Rotate(0.0f, 0.0f, 90.0f);
            }
            else
            {
                //Acomodo la esfera si estaba rotada y esto NO es un Equi-Angular Cubemap
                if (!equiangularCubemap && qu == rotated)
                {
                    sphere.transform.Rotate(0.0f, 0.0f, 90.0f);
                }
                    
            }

            // Asigno RenderTexture y video al VideoPlayer
            VideoPlayer videoPlayer = GameObject.Find("VideoPlayer").GetComponent<VideoPlayer>();
            videoPlayer.clip = video;
            //videoPlayer.videoSource = video;
            videoPlayer.targetTexture = renderTexture;
        }
        else
        {
            //Espero a cambiar de Escena, probablemente antes pongo la de carga
            //Asigno la textura
        }
    }
}