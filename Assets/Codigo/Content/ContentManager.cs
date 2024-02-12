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
			StartCoroutine(LoadContents());
		}

		private IEnumerator LoadContents()
		{
			Transform contentsTransform = transform.GetChild(1);
			// Cargar Imagenes
			Transform photosTransform = contentsTransform.GetChild(0);
			foreach (Transform photoTransform in photosTransform){
				Content photo = photoTransform.gameObject.GetComponent<Photo>();
				if(photo != null){
					contents.Add(photo);
				}else{
					Debug.LogWarning("El GameObject "+photoTransform.gameObject.name+" no tiene un script Photo");
				}
			}
			// Cargar Videos
			Transform videosTransform = contentsTransform.GetChild(1);
			foreach (Transform videoTransform in videosTransform){
				Content video = videoTransform.gameObject.GetComponent<Video>();
				if(video != null){
					contents.Add(video);
				}else{
					Debug.LogWarning("El GameObject "+videoTransform.gameObject.name+" no tiene un script Video");
				}
			}
			// Cargar Escenas 3D
			Transform scenesTransform = contentsTransform.GetChild(2);
			foreach (Transform sceneTransform in scenesTransform){
				Content scene = sceneTransform.gameObject.GetComponent<Escena>();
				if(scene != null){
					contents.Add(scene);
				}else{
					Debug.LogWarning("El GameObject "+sceneTransform.gameObject.name+" no tiene un script Escena");
				}
			}
			yield return null;
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
		/// Returns a Map with the keys being the content types (3D, video, photo),
		///  and the value being a list with the names of the available contents of
		///  that type.<br/> <br/>
		/// It doesn't include the scenes "main" or "SphereScene", as those
		/// aren't supposed to be directly accesed by the client.
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, List<string>> getContents()
		{
			Dictionary<string, List<string>> toReturnDict = new Dictionary<string, List<string>>();
			List<string> scenes = new List<string>();
			List<string> videos = new List<string>();
			List<string> photos = new List<string>();
			foreach(Content content in contents){
				if(content.nombre != "Main" && content.nombre != "SphereScene"){
					switch (content.getContentType()){
						case "3D":
							scenes.Add(content.nombre);
							break;

						case "video":
							videos.Add(content.nombre);
							break;

						case "photo":
							photos.Add(content.nombre);
							break;
					}
				}
			}

			bool hasSomething = false;
			if(scenes.Count > 0){
				toReturnDict.Add("3D",scenes);
				hasSomething = true;
			}
			if(videos.Count > 0){
				toReturnDict.Add("videos",videos);
				hasSomething = true;
			}
			if(photos.Count > 0){
				toReturnDict.Add("photos",photos);
				hasSomething = true;
			}

			if(hasSomething){
				return toReturnDict;
			}else{
				return null;
			}
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

		/// <summary>
		/// Stops content reproduction and returns to the "Main" scene.
		/// </summary>
		/// <returns></returns>
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

		/// <summary>
		/// pauses any currently playing content.
		/// Exactly what that means depends on the kind of
		/// content currently playing (image/video/scene).
		/// </summary>
		/// <returns></returns>
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