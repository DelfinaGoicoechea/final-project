using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Photo : Content
{
    //public Image photo; //Probablemente deba ser una textura, en vez de una Image
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
        GameObject sphere = ContentManager.Instance.getSphere();
        MeshRenderer meshR = sphere.GetComponent<MeshRenderer>();
        Material[] meshmaterials = meshR.materials;
        meshmaterials[0] = material;
        meshR.materials = meshmaterials;
    }
}