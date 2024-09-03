using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.Text;
using System.IO;
using SimpleHttpServer.Models;

namespace Contents{

	/// <summary>
	/// Clase encargada de gestionar la disponibilidad y reproduccion del contenido.
	/// Es SINGLETON y no muere en toda la ejecucion.
	/// </summary>
	public class ContentManager : MonoBehaviour
	{
		public AudioSource audioSource;
		public List<Content> contents;
		private List<AudioContainer> audios = new List<AudioContainer>();
		private GameObject sphere;
		private Content currentlyPlaying;
		private float configuredVolume = 1.0f;
		private FileSystemContentLoader fsLoader;

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
				fsLoader = new FileSystemContentLoader();
				Instance = this;
			}
			DontDestroyOnLoad(this.gameObject);
		}

		// Start is called before the first frame update
		void Start()
		{
			StartCoroutine(loadContents());
		}


		private void loadCustomizations(){
			string cambiosFileLocation = $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}cambios.txt";
			if(File.Exists(cambiosFileLocation)){
				string[] cambios = File.ReadAllLines(cambiosFileLocation);
				List<string> linesToRemove = new List<string>();
				foreach (string cambio in cambios){
					string result;
					Debug.Log("cambio: " + cambio);
					DataSavedCustomizations jsonCambios = DataSavedCustomizations.CreateFromJSON(cambio);
					switch(jsonCambios.contentType)
					{
						case "escena":
						    DataCustomizeEscena dce = DataCustomizeEscena.CreateFromJSON(jsonCambios.cambios);
							result = customizeEscena(dce.oldName, dce.newName, dce.newAudio, false);
							if (result == $"El contenido {dce.oldName} no existe"){
								linesToRemove.Add(cambio);
							}
							break;
						case "photo":
						    DataCustomizePhoto dcph = DataCustomizePhoto.CreateFromJSON(jsonCambios.cambios);
							result  = customizePhoto(dcph.oldName, dcph.newName, dcph.newAudio, false);
							if (result == $"El contenido {dcph.oldName} no existe"){
								linesToRemove.Add(cambio);
							}
							break;
						case "video":
							DataCustomizeVideo dcv = DataCustomizeVideo.CreateFromJSON(jsonCambios.cambios);
							result = customizeVideo(dcv.oldName, dcv.isEquiangular, dcv.changedRepresentation, dcv.newName, dcv.newAudio, false);
							if (result == $"El contenido {dcv.oldName} no existe"){
								linesToRemove.Add(cambio);
							}
							break;
						case "extApp":
							DataCustomizeApp dca = DataCustomizeApp.CreateFromJSON(jsonCambios.cambios);
							result = customizeExtApp(dca.oldName, dca.newName, dca.packageName, false);
							if (result == $"El contenido {dca.oldName} no existe"){
								linesToRemove.Add(cambio);
							}
							break;
					}
				}

				// Si se debe eliminar alguna linea, lo reflejo en el archivo almacenado
				if(linesToRemove.Count > 0){
					StreamWriter streamWriter = File.CreateText(cambiosFileLocation);
					foreach (string cambio in cambios){
						if (!linesToRemove.Contains(cambio)){
							streamWriter.WriteLine(cambio);
						}
					}
					streamWriter.Close();
				}
			}
		}


		private void loadAudiosAndCustomizations()
		{
			Transform audiosTransform = transform.GetChild(2);
			// Cargar audios definidos en tiempo de compilacion
			foreach (Transform audioTransform in audiosTransform){
				AudioContainer audioContainer = audioTransform.gameObject.GetComponent<AudioContainer>();
				if(audioContainer != null)
				{
					audios.Add(audioContainer);
				}
				else
				{
					Debug.LogWarning("El GameObject "+audioTransform.gameObject.name+" no tiene un script AudioContainer");
				}
			}

			// Cargar audios cargados con adb desde Flutter
			List<AudioContainer> audiosToAdd = fsLoader.LoadAudios(audios);
			foreach(AudioContainer audioContainer in audiosToAdd) {
				audios.Add(audioContainer);
			}

			// Cargar personalizaciones a los contenidos existentes
			loadCustomizations();
			return;
		}


		private void loadDisables()
		{
			string disablesFileLocation = $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}disables.txt";
            if (File.Exists(disablesFileLocation)) {
				var disables = File.ReadLines(disablesFileLocation);
				foreach (string contentToDisable in disables){
					deleteContent(contentToDisable, true);
				}
			}
		}


		private IEnumerator loadContents()
		{
			Transform contentsTransform = transform.GetChild(1);
			// Cargar Imagenes definidas en tiempo de compilacion
			Transform photosTransform = contentsTransform.GetChild(0);
			foreach (Transform photoTransform in photosTransform){
				Content photo = photoTransform.gameObject.GetComponent<Content>();
				if(photo != null)
				{
					contents.Add(photo);
				}
				else
				{
					Debug.LogWarning("El GameObject "+photoTransform.gameObject.name+" no tiene un script Photo");
				}
			}

			// Cargar Videos definidos en tiempo de compilacion
			Transform videosTransform = contentsTransform.GetChild(1);
			foreach (Transform videoTransform in videosTransform){
				Content video = videoTransform.gameObject.GetComponent<Video>();
				if(video != null){
					contents.Add(video);
				}else{
					Debug.LogWarning("El GameObject "+videoTransform.gameObject.name+" no tiene un script Video");
				}
			}

			// Cargar Escenas 3D definidas en tiempo de compilacion
			Transform scenesTransform = contentsTransform.GetChild(2);
			foreach (Transform sceneTransform in scenesTransform){
				Content scene = sceneTransform.gameObject.GetComponent<Escena>();
				if(scene != null){
					contents.Add(scene);
				}else{
					Debug.LogWarning("El GameObject "+sceneTransform.gameObject.name+" no tiene un script Escena");
				}
			}

			// Cargar caminos definidos en tiempo de compilacion
			Transform walksTransform = contentsTransform.GetChild(3);
			foreach (Transform walkTransform in walksTransform){
				Content walk = walkTransform.gameObject.GetComponent<Walk>();
				if(walk != null){
					contents.Add(walk);
				}else{
					Debug.LogWarning("El GameObject "+ walkTransform.gameObject.name+" no tiene un script Walk");
				}
			}

			// Desactivar contenidos ""eliminados"", porque no es posible eliminar
			// aquellos que fueron definidos en tiempo de compilacion.
			loadDisables();

			// Cargar Imagenes cargadas con adb desde Flutter
			List<PathPhoto> photosToAdd = fsLoader.LoadImages(contents);
			foreach(Photo photo in photosToAdd) {
				contents.Add(photo);
			}

			// Cargar Videos cargados con adb desde Flutter
			List<PathVideo> videosToAdd = fsLoader.LoadVideos(contents);
			foreach(Video video in videosToAdd) {
				contents.Add(video);
			}

			// Cargar Walks cargados con adb desde Flutter
			List<Walk> walksToAdd = fsLoader.LoadWalks();
			foreach (Walk walk in walksToAdd)
			{
				contents.Add(walk);
			}

			// Cargar ExternalApps cargadas con adb desde Flutter
			List<ExternalApp> appsToAdd = fsLoader.LoadExternalApps(contents);
			foreach(ExternalApp app in appsToAdd){
				contents.Add(app);
			}

			// Cargar Audios, y luego las personalizaciones a los contenidos existentes
			loadAudiosAndCustomizations();
			// TODO: chequear: ni loadAudios, ni loadCustomization son corutinas, asi que
			// deberia poder ejecutarlos a ambos aca sin anidar un llamado dentro del otro 
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
			}
		}

		public string customizeEscena(string oldName, string newName = null, string newAudio = null, bool saveChange = true)
		{
			Content content = contents.Find(x => x.nombre.Equals(oldName));
			if (content == null){
				string warning = $"El contenido {oldName} no existe";
				Debug.LogWarning(warning);
				return warning;
			}
			else{
				if(content.GetType() != typeof(Contents.Escena)){
					// Este chequeo nunca deberia causar problemas porque desde flutter ya se
					// sabe el tipo del contenido, pero mas vale prevenir que curar.
					string warning = $"El contenido {oldName} existe, pero el tipo no es Escena";
					Debug.LogWarning(warning);
					return warning;
				}
				Escena scene = (Escena) content;
				if (newName != null){
					Content contentNewName = contents.Find(x => x.nombre.Equals(newName));
					if (contentNewName != null && content != contentNewName){
						string warning = $"Ya existe un contenido llamado \"{newName}\"";
						Debug.LogWarning(warning);
						return warning;
					}
					scene.changeNombre(newName);
				}	
				if (newAudio != null){
					if(newAudio == ""){
						scene.soundContainer = null;
					}else{
						AudioContainer audioContainer = audios.Find(x => x.nombre.Equals(newAudio));
						if(audioContainer == null){
							string warning = $"El audio solicitado \"{newAudio}\" no existe";
							Debug.LogWarning(warning);
							return warning;
						}
						scene.soundContainer = audioContainer;
					}
				}
			}
			
			// Si realmente hubo cambios, los guardo
			if(saveChange && (newName != null || newAudio != null)){
				StringBuilder cambios = new StringBuilder("\n{\"contentType\":\"escena\",");
				cambios.Append($"\"cambios\":\"{{\\\"oldName\\\":\\\"{oldName}\\\",");
				if(newName != null)
					cambios.Append($"\\\"newName\\\":\\\"{newName}\\\",");
				if(newAudio != null)
					cambios.Append($"\\\"newAudio\\\":\\\"{newAudio}\\\",");
				cambios = cambios.Remove(cambios.Length-1, 1);
				cambios.Append("}\"}");
				string cambiosFileLocation = $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}cambios.txt";
				if(!File.Exists(cambiosFileLocation)){ 
					cambios.Remove(0,1); // Quito el salto de linea inicial (si es el primer cambio de la lista)
					File.Create(cambiosFileLocation).Dispose();
				} else {
					// si el archivo existe, pero esta vacio (por disables), tambien quito la linea inicial
					long fileLength = new FileInfo(cambiosFileLocation).Length;
					if (fileLength == 0) {
						cambios.Remove(0,1); 
					}
				}

				using (StreamWriter streamWriter = File.AppendText(cambiosFileLocation)){
					streamWriter.Write(cambios.ToString());
				}
			}
			return "";
		}

		public string customizePhoto(string oldName, string newName = null, string newAudio = null, bool saveChange = true)
		{
			Content content = contents.Find(x => x.nombre.Equals(oldName));
			if (content == null){
				string warning = $"El contenido {oldName} no existe";
				Debug.LogWarning(warning);
				return warning;
			}
			else{
				Type tipoContent = content.GetType();
				if(content.GetType() != typeof(Contents.Photo) && !tipoContent.IsSubclassOf(typeof(Contents.Photo))){
					// Este chequeo nunca deberia causar problemas porque desde flutter ya se
					// sabe el tipo del contenido, pero mas vale prevenir que curar.
					string warning = $"El contenido {oldName} existe, pero el tipo no es Photo";
					Debug.LogWarning(warning);
					return warning;
				}
				Photo photo = (Photo) content;
				if (newName != null){
					Content contentNewName = contents.Find(x => x.nombre.Equals(newName));
					if (contentNewName != null && content != contentNewName){
						string warning = $"Ya existe un contenido llamado \"{newName}\"";
						Debug.LogWarning($"Ya existe un contenido llamado \"{newName}\"");
						return warning;
					}
					photo.changeNombre(newName); // Util porque renombra thumbnails
				}	
				if (newAudio != null){
					if(newAudio == ""){
						photo.soundContainer = null;
					}else{
						AudioContainer audioContainer = audios.Find(x => x.nombre.Equals(newAudio));
						if(audioContainer == null){
							string warning = $"El audio solicitado \"{newAudio}\" no existe";
							Debug.LogWarning(warning);
							return warning;
						}
						photo.soundContainer = audioContainer;
					}
				}
			}

			// Si realmente hubo cambios, los guardo
			if(saveChange && (newName != null || newAudio != null)){
				StringBuilder cambios = new StringBuilder("\n{\"contentType\":\"photo\",");
				cambios.Append($"\"cambios\":\"{{\\\"oldName\\\":\\\"{oldName}\\\",");
				if(newName != null)
					cambios.Append($"\\\"newName\\\":\\\"{newName}\\\",");
				if(newAudio != null)
					cambios.Append($"\\\"newAudio\\\":\\\"{newAudio}\\\",");
				cambios = cambios.Remove(cambios.Length-1, 1);
				cambios.Append("}\"}");
				string cambiosFileLocation = $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}cambios.txt";
				if(!File.Exists(cambiosFileLocation)){
					cambios.Remove(0,1); // Quito el salto de linea inicial (si es el primer cambio de la lista)
					File.Create(cambiosFileLocation).Dispose();
				} else {
					// si el archivo existe, pero esta vacio (por disables), tambien quito la linea inicial
					long fileLength = new FileInfo(cambiosFileLocation).Length;
					if (fileLength == 0) {
						cambios.Remove(0,1); 
					}
				}

				using (StreamWriter streamWriter = File.AppendText(cambiosFileLocation)){
					streamWriter.Write(cambios.ToString());
				}
			}
			return "";
		}

		public string customizeVideo(string oldName, bool isEquiangular, bool changedRepresentation, string newName = null, string newAudio = null, bool saveChange = true)
		{
			Content content = contents.Find(x => x.nombre.Equals(oldName));
			if (content == null){
				string warning = $"El contenido {oldName} no existe";
				Debug.LogWarning(warning);
				return warning;
			}
			else
			{
				Type tipoContent = content.GetType();
				if(!tipoContent.IsSubclassOf(typeof(Contents.Video)))
				{
					// Este chequeo nunca deberia causar problemas porque desde flutter ya se
					// sabe el tipo del contenido, pero mas vale prevenir que curar.
					string warning = $"El contenido {oldName} existe, pero el tipo no es Video";
					Debug.LogWarning(warning);
					return warning;
				}
				Video video = (Video) content;
				if (changedRepresentation){
					video.isEquiangularCubemap = isEquiangular;
				}
					
				if (newName != null){
					Content contentNewName = contents.Find(x => x.nombre.Equals(newName));
					if (contentNewName != null && content != contentNewName){
						string warning = $"Ya existe un contenido llamado \"{newName}\"";
						Debug.LogWarning(warning);
						return warning;
					}
					video.changeNombre(newName);
				}
				if (newAudio != null){
					if(newAudio == ""){
						video.soundContainer = null;
					}else{
						AudioContainer audioContainer = audios.Find(x => x.nombre.Equals(newAudio));
						if(audioContainer == null){
							string warning = $"El audio solicitado \"{newAudio}\" no existe";
							Debug.LogWarning(warning);
							return warning;
						}
						video.soundContainer = audioContainer;
					}
				}
			}

			// Si realmente hubo cambios, los guardo
			if(saveChange && (newName != null || newAudio != null || changedRepresentation)){
				StringBuilder cambios = new StringBuilder("\n{\"contentType\":\"video\",");
				cambios.Append($"\"cambios\":\"{{\\\"oldName\\\":\\\"{oldName}\\\",");
				if(newName != null)
					cambios.Append($"\\\"newName\\\":\\\"{newName}\\\",");
				if(newAudio != null)
					cambios.Append($"\\\"newAudio\\\":\\\"{newAudio}\\\",");
				if(changedRepresentation){
					string isEquiangular_lowercase = $"{isEquiangular}".ToLower();
					cambios.Append($"\\\"isEquiangular\\\":{isEquiangular_lowercase},");
					cambios.Append($"\\\"changedRepresentation\\\":true,");
				}
				cambios = cambios.Remove(cambios.Length-1, 1);
				cambios.Append("}\"}");
				string cambiosFileLocation = $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}cambios.txt";
				if(!File.Exists(cambiosFileLocation)){
					cambios.Remove(0,1); // Quito el salto de linea inicial (si es el primer cambio de la lista)
					File.Create(cambiosFileLocation).Dispose();
				} else {
					// si el archivo existe, pero esta vacio (por disables), tambien quito la linea inicial
					long fileLength = new FileInfo(cambiosFileLocation).Length;
					if (fileLength == 0) {
						cambios.Remove(0,1); 
					}
				}

				using (StreamWriter streamWriter = File.AppendText(cambiosFileLocation)){
					streamWriter.Write(cambios.ToString());
				}
			}
			return "";
		}

		public string customizeExtApp(string oldName, string newName = null, string packageName = null, bool saveChange = true)
		{
			Content content = contents.Find(x => x.nombre.Equals(oldName));
			if (content == null){
				string warning = $"El contenido {oldName} no existe";
				Debug.LogWarning(warning);
				return warning;
			}
			else{
				if(content.GetType() != typeof(Contents.ExternalApp)){
					// Este chequeo nunca deberia causar problemas porque desde flutter ya se
					// sabe el tipo del contenido, pero mas vale prevenir que curar.
					string warning = $"El contenido {oldName} existe, pero el tipo no es Aplicacion";
					Debug.LogWarning(warning);
					return warning;
				}
				ExternalApp extApp = (ExternalApp) content;
				if (newName != null){
					Content contentNewName = contents.Find(x => x.nombre.Equals(newName));
					if (contentNewName != null && content != contentNewName){
						string warning = $"Ya existe un contenido llamado \"{newName}\"";
						Debug.LogWarning(warning);
						return warning;
					}
					extApp.changeNombre(newName);
				}	
				if (packageName != null){
					if(packageName == ""){
						string warning = $"El packagenName de {extApp.nombre} no puede estar vacio";
						Debug.LogWarning(warning);
						return warning;
					}else{
						extApp.changePackageName(packageName);
					}
				}
			}
			
			// Los cambios ya se guardan en changeNombre() y changePackageName()
			return "";
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
			foreach(Content content in contents){
				if(content.nombre != "Main" && content.nombre != "SphereScene"){
					content.addToDictionary(toReturnDict);
				}
			}

			if(toReturnDict.Count > 0){
				return toReturnDict;
			}else{
				return null;
			}
		}

		public Dictionary<string,string> getContentConfig(string name){
			Content content = contents.Find(x => x.nombre.Equals(name));
			if (content != null){
				Dictionary<string,string> config = content.getCurrentConfig();
				return config;
			}
			return null;
		}

		/// <summary>
		/// Returns a List with the names of the available Audios for scenes.
		/// </summary>
		/// <returns>List<string></returns>
		public List<string> getAudios()
		{
			List<string> availableAudiosList = new List<string>();
			foreach (AudioContainer audioContainer in audios)
			{
				availableAudiosList.Add(audioContainer.nombre);
			}
			return availableAudiosList;
		}

		/// <summary>
		/// Adds an entry for an external app (like Google Earth and such).
		/// </summary>
		/// <param name="scene"></param>
		/// <returns></returns>
		public bool addExternalApp(string visibleName, string packageName){
			ExternalApp externalApp = fsLoader.LoadSingleExternalApp(visibleName, packageName, contents);
			
			if (externalApp != null){
				contents.Add(externalApp);
				return true;
			}
			return false; // hubo problema en la creacion de la app 
		}

		/// <summary>
		/// Este metodo sirve para que la imagen que fue pasada usando adb, ahora
		/// pueda ser cargada al ContentManager sin la necesidad de reiniciar la aplicacion.
		/// </summary>
		/// <param name="filepath"></param>
		/// <returns></returns>
		public bool addPathImage(string filepath){
			FileInfo fileInfo = new FileInfo(filepath);
			string filename = fileInfo.Name.Split('.')[0];
			PathPhoto photo = fsLoader.LoadSingleImage(fileInfo, contents);
			if(photo != null){
				contents.Add(photo);
				return true;
			}
			return false; // extension de archivo incompatible
		}

		/// <summary>
		/// Este metodo sirve para que la imagen que fue pasada usando adb, ahora
		/// pueda ser cargada al ContentManager sin la necesidad de reiniciar la aplicacion.
		/// </summary>
		/// <param name="filepath"></param>
		/// <returns></returns>
		public bool addPathVideo(string filepath){
			FileInfo fileInfo = new FileInfo(filepath);
			string filename = fileInfo.Name.Split('.')[0];
			PathVideo video = fsLoader.LoadSingleVideo(fileInfo, contents);
			if(video != null){
				contents.Add(video);
				return true;
			}
			return false; // extension de archivo incompatible
		}


		/// <summary>
		/// Este metodo sirve para que el audio que fue pasado usando adb, ahora
		/// pueda ser cargado al ContentManager sin la necesidad de reiniciar la aplicacion.
		/// </summary>
		/// <param name="filepath"></param>
		/// <returns></returns>
		public bool addPathAudio(string filepath){
			FileInfo fileInfo = new FileInfo(filepath);
			string filename = fileInfo.Name.Split('.')[0];
			
			AudioContainer audio = audios.Find(x => x.nombre.Equals(filename));
			if(audio != null){
				// TODO: Deberia renombrarlo y proseguir, porque al reiniciar la app
				// va a intentar cargarlo y crearlo igualmente.
				return false;
			}
			AudioContainer audioContainer = fsLoader.LoadSingleAudio(fileInfo, audios);
			if(audioContainer != null){
				audios.Add(audioContainer);
				return true;
			}
			return false; // extension de archivo incompatible
		}
		
		/// <summary>
		/// Takes the name of a Content as an input, and returns its thumbnail as a Texture2D.
		/// </summary>
		/// <param name="scene"></param>
		/// <returns></returns>
		public Texture2D getThumbnail(string scene){
			Debug.Log("La escena que se carga es: " + scene);
			Content content = contents.Find(x => x.nombre.Equals(scene));
			if (content == null){
				Debug.LogWarning("No se encontro el Contenido: " + scene);
				return null;
			}
			return content.thumbnail;
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
				if(this.currentlyPlaying != null && this.currentlyPlaying != content){
					// me aseguro de parar la reproduccion del anterior,
					// antes de reproducir el nuevo contenido
					currentlyPlaying.stop();
				}
				this.currentlyPlaying = content;
                Time.timeScale = 1.0f;
                content.play();
				StartCoroutine(fadeIn(2.5f));
				return true;
			}
			Debug.LogWarning("No se encontro el Contenido: " + scene);
			return false;
		}

		public bool deleteContent(string scene, bool fromLoadContents = false){
			Content content = contents.Find(x => x.nombre.Equals(scene));
			if (content != null)
			{
				if(this.currentlyPlaying != null && this.currentlyPlaying == content) {
					// Me aseguro de frenar la reproducción, porque lo voy a eliminar
					currentlyPlaying.stop();
					this.stop();
				}

				// si vengo de (LoadContents -> LoadDisables) => salteo el paso de agregar 
				// al archivo de disables, y directamente asigno deleteExitoso = true;
				bool deleteExitoso = (fromLoadContents) ? true : content.delete();

				if (deleteExitoso){
					bool listRemoveExitoso = contents.Remove(content);
					if (listRemoveExitoso) {
						Destroy(content.gameObject);
						return true;
					}
					return false;
				}
				Debug.LogWarning("Ocurrio un error durante content.delete(), de: " + scene);
				return false;
			}
			Debug.LogWarning("No se encontro el Contenido: " + scene);
			return false;
		}

		private IEnumerator fadeIn(float duration)
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
		/// </summary>
		/// <returns></returns>
		public GameObject getSphere()
		{
			return sphere;
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
			// Solo voy a PlayContent si me tiran Stop y actualmente no estoy en la escena Main
			bool alreadyInMain = this.currentlyPlaying == null || this.currentlyPlaying != null && this.currentlyPlaying.nombre == "Main";
			if (!alreadyInMain)
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
			float duration = 2.5f;
            if (currentlyPlaying != null)
            {
                currentlyPlaying.pause();
            }
            while (currentTime < duration)
			{
				currentTime += Time.fixedDeltaTime;
				audioSource.volume = Mathf.Lerp(start, end, currentTime / duration);
				yield return new WaitForFixedUpdate();
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
				if (videoPlayer.clip != null)
				{
					if (videoPlayer.clip.audioTrackCount > 0)
					{
						videoPlayer.SetDirectAudioVolume(0, 0);
					}
				}
			}
		}

		public string getPlayingName(){
			if (currentlyPlaying != null){
				return currentlyPlaying.nombre;
			}
			else{
				return null;
			}
		}
	}
}