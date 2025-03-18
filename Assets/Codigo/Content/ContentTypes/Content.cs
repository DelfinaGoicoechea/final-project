using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
        protected bool isPaused = false;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public virtual void play(Dictionary<string, object> config){
            // Paro la reproduccion del sonido de cualquier Content previo
            AudioSource audioSource = ContentPlayer.Instance.audioSource;
            audioSource.Pause();
            if (isPaused)
            {
                isPaused = false;
                if (this.soundContainer != null){
                    audioSource.Play();
                }
                return;
            }

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
            isPaused = true;
        }

        public virtual void stop()
        {
            isPaused = false;
            AudioSource audioSource = ContentPlayer.Instance.audioSource;
            if (this.soundContainer != null){
                audioSource.Stop();
                soundContainer.UnloadAudio();
            }
        }

        public virtual Dictionary<string,object> getCurrentConfig(){
            Dictionary<string,object> config = new Dictionary<string,object>();
            config["name"] = this.nombre;
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
            // antes  tenia sentido xq se implementaba el codigo para eliminar tags,
            // pero lo dejo xq igualmente puede ser util en el futuro
            return true;
        }


        protected void storeNewCustomization(Dictionary<string, object> cambios) {
            string cambiosFileLocation = $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}cambios.txt";

            bool hasNewName = cambios.ContainsKey("newName");           
            // Guardo las personalizaciones
            
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
            // Guardo la fecha del cambio
            string dateFileLocation = $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}lastChangeDate.txt";
            string dateToStore = DateTime.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture);
            if(!File.Exists(dateFileLocation)){
                File.Create(dateFileLocation).Dispose();
            }
            File.WriteAllText(dateFileLocation, dateToStore);
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