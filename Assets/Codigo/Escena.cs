using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Escena: Content
{
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
        //yet to be implemented}
        SceneManager.LoadScene(nombre);
        AudioSource audioSource = ContentManager.Instance.audioSource;
        audioSource.Pause();
        if (this.sound != null)
        {
            audioSource.clip = this.sound;
            audioSource.Play();
        }
    }
}