using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Newtonsoft.Json;

namespace Contents{

	/// <summary>
	/// Clase encargada de gestionar la disponibilidad y reproduccion del contenido.
	/// Es SINGLETON y no muere en toda la ejecucion.
	/// </summary>
	public class ContentManager : MonoBehaviour
	{
		public List<Content> contents;
		private List<AudioContainer> audios = new List<AudioContainer>();
		private GameObject sphere;
		private FileSystemContentLoader fsLoader;
		public DateTime lastUpdateTime = DateTime.UtcNow;
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
			string dateFileLocation = $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}lastChangeDate.txt";
			if (File.Exists(dateFileLocation)) {
				// No es necesario leer el contenido, ya que me alcanza con la ultima fecha de escritura
				lastUpdateTime = File.GetLastWriteTimeUtc(dateFileLocation);
			}
		}


		private void loadCustomizations(string contentCreationTime) { // Compile/Execution
			string cambiosFileLocation = $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}cambios.txt";
			if(File.Exists(cambiosFileLocation)){
				string[] cambiosFileContent = File.ReadAllLines(cambiosFileLocation);
				// Deserealizo en "customizations" el contenido de cambios.txt
				Dictionary<string, List<Dictionary<string, object>>> customizations = JsonConvert.DeserializeObject<Dictionary<string, List<Dictionary<string, object>>>>(cambiosFileContent[0]);
				//	cambios.txt tiene el formato:
				//  {
				//   	"Compile": [{"oldName":"bosque", "newName":"bosque2"}, ...],
				//    	"Execution": [{"oldName":"IMG_24", "newName":"Foto cargada"}, ...]
				// 	}
				// 	Esto es para evitar algunos problemas que surgen al no poder eliminar o renombrar
				//  de forma permanente a los contenidos  definidos en tiempo de compilación.
				if (customizations.ContainsKey(contentCreationTime)) { // Compile/Execution
					List<Dictionary<string, object>> cambios = customizations[contentCreationTime];
					List<Dictionary<string, object>> cambiosToRemove = new List<Dictionary<string, object>>();
					foreach (Dictionary<string, object> cambio in cambios) {
						string result = customizeContent(cambio, false);
						if (result == $"El contenido {(string)cambio["oldName"]} no existe"){
							cambiosToRemove.Add(cambio);
						}
					}

					// Si se debe eliminar alguna linea, lo reflejo en el archivo almacenado
					foreach (Dictionary<string, object> cambio2Remove in cambiosToRemove) {
						cambios.Remove(cambio2Remove);
						string strCustomizations = JsonConvert.SerializeObject(customizations);
						File.WriteAllText(cambiosFileLocation, strCustomizations);
					}
				}				
			}
		}


		private void loadAudios()
		{
			Transform audiosTransform = transform.GetChild(1);
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

		private void loadTags()
		{
			string tagsFileLocation = $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}tags.txt";
			if (File.Exists(tagsFileLocation)) {
				string tagsFileContent = File.ReadAllLines(tagsFileLocation)[0];
                Dictionary<string, List<string>> tagsContent = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(tagsFileContent);
				foreach (KeyValuePair<string, List<string>> tags in tagsContent) {
					Content content = contents.Find(x => x.nombre.Equals(tags.Key));
					if (content != null){
						content.tags = new List<string> (tags.Value);
					}
				}
			}
		}


		private IEnumerator loadContents()
		{
			Transform contentsTransform = transform.GetChild(0);
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

			// Cargo los audios, porque puede que formen parte de las personalizaciones
			loadAudios();

			// Cargar personalizaciones a los contenidos definidos en tiempo de compilacion
			loadCustomizations("Compile");

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

			// Cargo las personalizaciones a los contenidos definidos en tiempo de ejecucion.
			loadCustomizations("Execution");
			loadTags();
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

		public Content getContent(string contentName){
			return contents.Find(x => x.nombre.Equals(contentName));
		}

		public string customizeContent(Dictionary<string, object> cambios, bool saveChange = true){
			// Chequeo eue el contenido a personalizar exista
			string oldName = (string)cambios["oldName"];
			Content contentOldName = contents.Find(x => x.nombre.Equals(oldName));
			if (contentOldName == null){
				string warning = $"El contenido {oldName} no existe";
				Debug.LogWarning(warning);
				return warning;
			}

			// Chequeo que el nuevo nombre todavia no exista
			bool hasNewName = cambios.ContainsKey("newName");
			if (hasNewName){
				string newName = (string) cambios["newName"];
				Content contentNewName = contents.Find(x => x.nombre.Equals(newName));
				if (contentNewName != null && contentOldName != contentNewName){
					string warning = $"Ya existe un contenido llamado \"{newName}\"";
					Debug.LogWarning(warning);
					return warning;
				}
			}
			lastUpdateTime = DateTime.UtcNow;
			// personalizo el contenido
			return contentOldName.customize(cambios, audios, saveChange);
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

		/// <summary>
		/// Retorna la configuracion actual de un contenido, a partir de su nombre.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public Dictionary<string,object> getCurrentContentConfig(string name){
			Content content = contents.Find(x => x.nombre.Equals(name));
			if (content != null){
				Dictionary<string,object> config = content.getCurrentConfig();
				return config;
			}
			return null;
		}

		// TODO: VER CoMO OBTENER Y CAMBIAR LAS CONFIGURACIONES DE ESCENAS DESDE FLUTTER

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
			// LoadSingleAudio renombra en caso de que fuera necesario
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
			Content content = contents.Find(x => x.nombre.Equals(scene));
			if (content == null){
				Debug.LogWarning("No se encontro el Contenido: " + scene);
				return null;
			}
			return content.thumbnail;
		}

		public bool deleteContent(string scene, bool fromLoadContents = false){
			Content content = contents.Find(x => x.nombre.Equals(scene));
			if (content != null)
			{
				ContentPlayer cPlayer = ContentPlayer.Instance;
				if(cPlayer.currentlyPlaying != null && cPlayer.currentlyPlaying == content) {
					// Me aseguro de frenar la reproducción, porque lo voy a eliminar
					cPlayer.currentlyPlaying.stop();
					Content mainScene = getContent("Main");
					cPlayer.stop(mainScene); // TODO: Ver por que no se usa solo cPlayer.stop()
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

		/// <summary>
		/// returns the sphere used to project a 360 image/video.
		/// </summary>
		/// <returns></returns>
		public GameObject getSphere()
		{
			return sphere;
		}
	}
}