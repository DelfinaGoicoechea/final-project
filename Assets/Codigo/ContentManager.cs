using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ContentManager : MonoBehaviour
{
	public List<Content> contents;
	private GameObject sphere;

	public static ContentManager Instance { get; private set; }
	private void Awake()
	{
		// If there is an instance, and it's not me, delete myself.
		DontDestroyOnLoad(this.gameObject);
		if (Instance != null && Instance != this)
		{
			Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		//or else
		this.playContent("mar");
		IEnumerator coroutine = WaitAndVideo(10.0f);
		StartCoroutine(coroutine);

	}

	private IEnumerator WaitAndVideo(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		this.playContent("playa1");
	}

	// Update is called once per frame
	void Update()
	{

	}

	void OnEnable()
	{
		Debug.Log("OnEnable called");
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	// called second
	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		Debug.Log("OnSceneLoaded: " + scene.name);
		Debug.Log(mode);
		if (SceneManager.GetActiveScene().name == "SphereScene")
		{
			sphere = GameObject.Find("ProjectionSphere");
			Debug.Log("Entre a encontrar la esfera");
			//Acá tengo que chequear, si estoy en SphereScene, busco la esfera
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
			content.play();
			return true;
		}
		Debug.LogWarning("No se encontró el Contenido: " + scene);
		return false;
	}

	public GameObject getSphere()
	{
		return sphere;
	}
}