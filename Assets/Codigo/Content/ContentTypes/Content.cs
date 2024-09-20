using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using Newtonsoft.Json;

namespace Contents{
    
    /// <summary>
    /// Tipo abstracto de contenido, luego extendido por Fotos, Videos y Escenas 3D.
    /// </summary>
    public abstract class Content : MonoBehaviour
    {
        public Texture2D thumbnail;
        public AudioContainer soundContainer;
        
        [Tooltip("Nombre a mostrar a los usuarios")]
        public string nombre;

        [Tooltip(@"Define en que entrada de cambios.txt se va a guardar los cambios.
         En el editor tipicamente seria Compile")]
        public string customizationsEntry = "Compile";
        public List<string> tags = new List<string>();

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public virtual void play(){
            // Paro la reproduccion del sonido de cualquier Content previo
            AudioSource audioSource = ContentPlayer.Instance.audioSource;
            audioSource.Pause();

            if (this.soundContainer != null)
            {
                // Es necesario pasar una funcion que asigne y comience la reproduccion (assignmentAuxFunc),
                // porque soundContainer.LoadPlayAudio() puede ejecutar la obtencion del
                // sonido de forma asincrona (PathAudioContainer), por lo que no está asegurado
                // que el audio esté cargado para darle play luego de LoadPlayAudio().
                Action assignmentAuxFunc = () => {
                    audioSource.clip = this.soundContainer.sound;
                    audioSource.Play();
                };
                soundContainer.LoadPlayAudio(assignmentAuxFunc);
            }
        }

        /// <summary>
        /// Agrega el nombre del contenido a un Diccionario
        /// que contiene listas de nombres de contenidos
        /// </summary>
        /// <param name="dictio"></param>
        public abstract void addToDictionary(Dictionary<string, List<string>> dictio);

        /// <summary>
        /// Pauses any music plying in the scene
        /// </summary>
        public virtual void pause()
        {
            // TODO: audioSource se podria pausar desde pause() en contentManager,
            // estas lineas parecen ser innecesarias y sobrecomplicar el asunto.
            AudioSource audioSource = ContentPlayer.Instance.audioSource;
            if (this.soundContainer != null){
                audioSource.Pause();
            }
        }

        public virtual void stop()
        {
            AudioSource audioSource = ContentPlayer.Instance.audioSource;
            if (this.soundContainer != null){
                audioSource.Stop();
                soundContainer.UnloadAudio();
            }
        }

        public virtual Dictionary<string,object> getCurrentConfig(){
            Dictionary<string,object> config = new Dictionary<string,object>();
            config["name"] = this.nombre;
            config["tags"] = this.tags;
            if (soundContainer != null){
                config["sound"] = soundContainer.nombre;
            }
            return config;
        }

        public virtual bool changeNombre(string newName){
            this.nombre = newName;
            return true;
        }

        public virtual bool delete() {
            string tagsFileLocation = $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}tags.txt";
            if (File.Exists(tagsFileLocation)) {
                string tagsFileContent = File.ReadAllLines(tagsFileLocation)[0];
                Dictionary<string, List<string>> tagsContent = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(tagsFileContent);
                // Si hay tags para este Content, los elimino.
                if(tagsContent.ContainsKey(this.nombre)){
                    tagsContent.Remove(this.nombre);
                    string strTags = JsonConvert.SerializeObject(tagsContent);
			        File.WriteAllText(tagsFileLocation, strTags);
                }
            }
            return true;
        }

        protected void storeTagsChanges (Dictionary<string, object> cambios, bool hasNewTags, bool hasNewName) {
            string tagsFileLocation = $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}tags.txt";
            Dictionary<string, List<string>> tagsContent;
            // ¿existe tags.txt?
            if (!File.Exists(tagsFileLocation)) { // No existe, entonces lo creo
                File.Create(tagsFileLocation).Dispose();
                tagsContent = new Dictionary<string, List<string>>();
            } else { // Existe, entonces lo cargo a tagsContent
                string tagsFileContent = File.ReadAllLines(tagsFileLocation)[0];
                tagsContent = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(tagsFileContent);
            }
            
            bool hadTags = tagsContent.ContainsKey( (string)cambios["oldName"] );
            if(hasNewTags){
                if (hasNewName && hadTags) {
                    tagsContent.Remove( (string)cambios["oldName"] );
                }
                tagsContent[this.nombre] = this.tags;
                
            } else {
                // Si no hay nuevos tags, es porque se renombro el contenido. Entonces
                // si el contenido tenia tags, tengo que modificar la key en tags.txt
                if (hadTags) {
                    tagsContent.Remove( (string)cambios["oldName"] );
                    tagsContent[this.nombre] = this.tags;
                }
            }
            string strTags = JsonConvert.SerializeObject(tagsContent);
			File.WriteAllText(tagsFileLocation, strTags);
            cambios.Remove("tags"); // elimino para que no se guarden tambien en cambios.txt
        }

        protected void storeNewCustomization(Dictionary<string, object> cambios) {
            string cambiosFileLocation = $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}cambios.txt";
            bool hasNewTags = cambios.ContainsKey("tags");
            bool hasNewName = cambios.ContainsKey("newName");
            // Guardo modificaciones relacionadas con los tags
            if( hasNewTags || hasNewName ){
                // 3 posibilidades:
                //    1. se modificaron tags y hay que guardarlo en tags.txt
                //    2. se cambio el nombre y puede que se deba cambiar una key en tags.txt
                //    3. todas las anteriores
                storeTagsChanges(cambios, hasNewTags, hasNewName);
            }
            
            // Guardo las demas personalizaciones
            
            // el archivo guarada: 
            //     Dict<str, List< Dict<str,obj> >>
            // Ejemplo:
            //     {"Compile" : [ {"oldName": "bosque", "newName":"Desierto"} ]}
            Dictionary<string, List<Dictionary<string, object>>> fileContent;
            if(!File.Exists(cambiosFileLocation)){
                File.Create(cambiosFileLocation).Dispose();
                fileContent = new Dictionary<string, List<Dictionary<string, object>>>();
                // Agrego la entry
                fileContent[this.customizationsEntry]  = new List<Dictionary<string, object>> {cambios};
            } else {
                string cambiosFileContent = File.ReadAllLines(cambiosFileLocation)[0];
                fileContent =  JsonConvert.DeserializeObject<Dictionary<string, List<Dictionary<string, object>>>>(cambiosFileContent);
                // File existe, ¿pero existe o no la Entry ("Compile"/"Execution")?
                if (!fileContent.ContainsKey(this.customizationsEntry)){
                    fileContent[this.customizationsEntry]  = new List<Dictionary<string, object>> {cambios};
                } else {
                    List<Dictionary<string, object>> listCambios = fileContent[this.customizationsEntry];
                    listCambios.Add(cambios);
                }
            }
            string strCustomizations = JsonConvert.SerializeObject(fileContent);
			File.WriteAllText(cambiosFileLocation, strCustomizations);
        }

        public virtual string customize(Dictionary<string, object> cambios, List<AudioContainer> audios, bool saveChange){
			// cambio el nombre
            if (cambios.ContainsKey("newName")){
				string newName = (string) cambios["newName"];
				this.changeNombre(newName);
			}

            // cambio el audio
			if (cambios.ContainsKey("newAudio")){
				string newAudio = (string) cambios["newAudio"];
				if(newAudio == ""){
					this.soundContainer = null;
				}else{
					AudioContainer audioContainer = audios.Find(x => x.nombre.Equals(newAudio));
					if(audioContainer == null){
						string warning = $"El audio solicitado \"{newAudio}\" no existe";
						Debug.LogWarning(warning);
						return warning;
					}
					this.soundContainer = audioContainer;
				}
			}

            // cambio los tags
            if (cambios.ContainsKey("tags")){
                this.tags = (List<string>) cambios["tags"];
            }

            // Las clases que heredan deben decidir si se deben guardar o no los cambios,
            // ya que cada una puede tener distintos criterios de decision.
            //
            // Ej: Todas las clases van a recibir un "saveChange" que viene desde 
            //     ContentManager e indica si se debe guardar el cambio o no. Pero su vez:
            //        1. Photo solo se guarda si tambien habia una entrada "newName" o "newAudio"
            //        2. Video ademas de name y audio tiene en cuenta la presencia de "isEquiangular".
            //
            // Por lo que cada una debe analizar lo suyo, y decidir que valor
            // asignar a "saveChange" antes de su llamado a base.customize(...)
            if (saveChange) {
                storeNewCustomization(cambios);
            }
            return "";
        }
    }
}