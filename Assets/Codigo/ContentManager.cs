using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ContentManager : MonoBehaviour
{
	public List<Content> contents;
	public GameObject sphere;
	//private Content currentlyPlaying;
	public AudioSource audioSource;

	public static ContentManager Instance { get; private set; }
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
		GameObject aSource = GameObject.Find("AudioSource");
		DontDestroyOnLoad(aSource);
        this.playContent("playa1");
        StartCoroutine(WaitAndVideo(5.0f, "SimpleNaturePackScene"));
		StartCoroutine(WaitAndVideo(10.0f, "bosque"));
        StartCoroutine(WaitAndVideo(15.0f, "venecia"));
        StartCoroutine(WaitAndVideo(20.0f, "playa1"));
        StartCoroutine(WaitAndVideo(30.0f, "venecia"));
    }

    private IEnumerator WaitAndVideo(float waitTime, string content)
	{
		yield return new WaitForSeconds(waitTime);
		this.playContent(content);
	}

	// Update is called once per frame
	void Update()
	{

	}

	void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	// called second
	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (SceneManager.GetActiveScene().name == "SphereScene")
		{
            // Busco ProjectionSphere
            sphere = GameObject.FindWithTag("ProjectionSphere");
		}
	}

	public List<Tuple<string, Image>> getContent()
	{
		return null;
	}

	public void addContent(Content content)
	{

	}

	public bool playContent(string scene)
	{
		Content content = contents.Find(x => x.nombre.Equals(scene));
		if (content != null)
		{
			//currentlyPlaying = content;
            content.play();
			return true;
		}
		Debug.LogWarning("No se encontr� el Contenido: " + scene);
		return false;
	}

	public GameObject getSphere()
	{
		return sphere;
	}

	public void stop()
	{
		//TODO: terminar reproduccion y llevar a la escena main
	}
}