using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Content : MonoBehaviour
{
    public Texture thumbnail;
    public AudioClip sound; //probablemente al setearlo tenga que hacer algun otro cambio si ya está en reproduccion
    public string nombre; //Puede ser util usar el atributo name que se hereda de más arriba

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public abstract void play();
    public virtual void pause()
    {
        // Does nothing, except in videos
    }
}
