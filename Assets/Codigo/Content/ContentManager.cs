using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace Contents{
	public class ContentManager : MonoBehaviour
	{
		public AudioSource audioSource;
		public List<Content> contents;
		private GameObject sphere;
		private GameObject sphereEAC;
		private Content currentlyPlaying;
		private float configuredVolume = 1.0f;
		

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
			//StartCoroutine(WaitAndVideo(6.0f, "iglesiaPh"));
		}

		// Solo usado para testing
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

		// Asignar esferas de proyeccion al estar en la escena adecuada 
		void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			if (SceneManager.GetActiveScene().name == "SphereScene")
			{
				// Busco ProjectionSphere
				sphere = GameObject.FindWithTag("ProjectionSphere");
				sphereEAC = GameObject.FindWithTag("ProjectionSphereEAC");
			}
		}

		/// <summary>
		/// Returns a list containing the name of the available contents.
		/// It doesn't include the scenes "main" or "SphereScene", as those
		/// aren't supposed to be directly accesed by the client.
		/// </summary>
		/// <returns></returns>
		public List<string> getContents()
		{
			List<string> toReturn = new List<string>();
			foreach(Content content in contents){
				if(content.nombre != "Main" && content.nombre != "SphereScene"){
					toReturn.Add(content.nombre);
				}
			}
			if(toReturn.Count > 0){
				return toReturn;
			}
			return null;
		}
		
		/// <summary>
		/// Takes the name of a Content as an input, and returns its thumbnail as a Texture2D.
		/// </summary>
		/// <param name="scene"></param>
		/// <returns></returns>
		public Texture2D getThumbnail(string scene){
			Content content = contents.Find(x => x.nombre.Equals(scene));
			if (content == null){
				Debug.LogWarning("No se encontro el Contenido: " + scene);
				return null;
			}
			return content.thumbnail;
		}

		public void addContent(Content content)
		{

		}

		/// <summary>
		/// Takes the name of a Content as an input, and starts playing it.
		/// </summary>
		/// <param name="scene"></param>
		/// <returns></returns>
		public bool playContent(string scene)
		{
			Content content = contents.Find(x => x.nombre.Equals(scene));
			if (content != null)
			{
				this.currentlyPlaying = content;
				content.play();
				StartCoroutine(FadeIn(5f));
				return true;
			}
			Debug.LogWarning("No se encontro el Contenido: " + scene);
			return false;
		}

		public IEnumerator FadeIn(float duration)
		{
			float currentTime = 0;
			float start = 0.0f;
			float end = configuredVolume;
			while (currentTime < duration)
			{
				currentTime += Time.fixedDeltaTime;
				audioSource.volume = Mathf.Lerp(start, end, currentTime / duration);
				yield return new WaitForFixedUpdate();
			}
			yield break;
		}

		/// <summary>
		/// returns the sphere used to project a 360 image/video.
		/// Check getSphereEAC for a special case.
		/// </summary>
		/// <returns></returns>
		public GameObject getSphere()
		{
			return sphere;
		}

		/// <summary>
		/// returns the sphere used to project an "EquiAngularCubemap" image/video.
		/// Usually found in 4K videos from Youtube.
		/// </summary>
		/// <returns></returns>
			public GameObject getSphereEAC()
		{
			return sphereEAC;
		}

		public IEnumerator stop()
		{
			float currentTime = 0.0f;
			float start = configuredVolume;
			float end = 0.0f;
			float duration = 5f;
			while (currentTime < duration)
			{
				currentTime += Time.fixedDeltaTime;
				audioSource.volume = Mathf.Lerp(start, end, currentTime / duration);
				yield return new WaitForFixedUpdate();
			}
			this.playContent("Main");
			yield return null;
		}

		public IEnumerator pause(){
			float currentTime = 0.0f;
			float start = configuredVolume;
			float end = 0.0f;
			float duration = 5f;
			while (currentTime < duration)
			{
				currentTime += Time.fixedDeltaTime;
				audioSource.volume = Mathf.Lerp(start, end, currentTime / duration);
				yield return new WaitForFixedUpdate();
			}
			if (currentlyPlaying != null){
				currentlyPlaying.pause();
			}
			yield return null;
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

			configuredVolume = vol;

			// El video solo esta presente en la escena de la esfera
			if(SceneManager.GetActiveScene().name == "SphereScene"){
				VideoPlayer videoPlayer = GameObject.FindWithTag("VideoPlayer").GetComponent<VideoPlayer>();
				if(videoPlayer.clip.audioTrackCount > 0){
					videoPlayer.SetDirectAudioVolume(0, 0);
				}
			}
		}

		public string getPlayingName(){
			return currentlyPlaying.nombre;
		}
	}
}