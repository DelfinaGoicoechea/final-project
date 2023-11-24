using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTextureSetter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer meshR = this.GetComponent<MeshRenderer>();
        Material material = meshR.materials[0];
        Texture renderTex = material.mainTexture;
        //Graphics.Blit(,renderTex)
        //maintex
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
