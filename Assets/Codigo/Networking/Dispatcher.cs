using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleHttpServer.Models;

public class Dispatcher : MonoBehaviour
{
    public static Dispatcher Instance { get; private set; }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        //contentManager cm = GameObject.FindWithTag("ContentManager").GetComponent<ContentManager>();
        //GameObject.FindWithTag("MetricsManager");
    }

    public HttpResponse Play(string scene, Func<bool, HttpResponse> callback){
        //TODO: llamar a cada cosa
        return null;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
