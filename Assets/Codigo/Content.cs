using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Content : MonoBehaviour
{
    public Image thumbnail;
    public AudioClip sound;

    // public AudioClip Audio { get => audio; set => audio = value; } //probablemente al setear tenga que hacer algun otro cambio
    // public Image Thumbnail { get => thumbnail; set => thumbnail = value; }

    // Start is called before the first frame update
    void Start()
    {
        //or else
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public abstract void play();
}
