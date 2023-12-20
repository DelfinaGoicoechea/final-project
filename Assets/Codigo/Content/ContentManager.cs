using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class ContentManager : MonoBehaviour
{
	public AudioSource audioSource;
	public List<Content> contents;
	private GameObject sphere;
	private GameObject sphereEAC;
	private Content currentlyPlaying;
	

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
        //this.playContent("bosque");
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
			sphereEAC = GameObject.FindWithTag("ProjectionSphereEAC");
		}
	}

	public List<Tuple<string, Texture>> getContent()
	{
		List<Tuple<string, Texture>> toReturn = new List<Tuple<string, Texture>>();
		foreach(Content content in contents){
			if(content.nombre != "Main" && content.nombre != "SphereScene"){
				Tuple<string, Texture> toAdd = new Tuple<string, Texture>(content.nombre, content.thumbnail);
				toReturn.Add(toAdd);
			}
		}
		if(toReturn.Count > 0){
			return toReturn;
		}
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
			this.currentlyPlaying = content;
            content.play();
			return true;
		}
		Debug.LogWarning("No se encontro el Contenido: " + scene);
		return false;
	}

	public GameObject getSphere()
	{
		return sphere;
	}

		public GameObject getSphereEAC()
	{
		return sphereEAC;
	}

	public void stop()
	{
		//TODO: terminar reproduccion y llevar a la escena main
		this.pause();
		SceneManager.LoadScene("Main");
	}

	public void pause(){
		if(currentlyPlaying != null){
			currentlyPlaying.pause();
		}
	}

	public void setVolume(float vol){
		if (vol > 100.0f){
			vol = 100.0f;
		}else if (vol < 0.0f){
			vol = 0.0f;
		}
		vol = vol / 100.0f;

		if (audioSource.clip != null){
			audioSource.volume = vol;
		}

		// El video solo esta presenta en la escena de la esfera
		if(SceneManager.GetActiveScene().name == "SphereScene"){
			VideoPlayer videoPlayer = GameObject.FindWithTag("VideoPlayer").GetComponent<VideoPlayer>();
			if(videoPlayer.clip.audioTrackCount > 0){
				videoPlayer.SetDirectAudioVolume(0, 0);
			}
		}
	}
}