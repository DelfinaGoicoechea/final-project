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
            AudioSource audioSource = ContentManager.Instance.audioSource;
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
            AudioSource audioSource = ContentManager.Instance.audioSource;
            if (this.soundContainer != null){
                audioSource.Pause();
            }
        }

        public virtual void stop()
        {
            AudioSource audioSource = ContentManager.Instance.audioSource;
            if (this.soundContainer != null){
                audioSource.Stop();
                soundContainer.UnloadAudio();
            }
        }

        public virtual Dictionary<string,string> getCurrentConfig(){
            Dictionary<string,string> config = new Dictionary<string,string>();
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

        public abstract bool delete ();

        protected void storeNewCustomization(string jsonCambios){
            string cambiosFileLocation = $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}cambios.txt";
            if(!File.Exists(cambiosFileLocation)){
                File.Create(cambiosFileLocation).Dispose();
            } else {
                // si el archivo existe pero no esta vacio, agrego un salto de linea al principio 
                long fileLength = new FileInfo(cambiosFileLocation).Length;
                if (fileLength > 0) {
                    jsonCambios = '\n'+jsonCambios;
                }
            }
            using (StreamWriter streamWriter = File.AppendText(cambiosFileLocation)){
                streamWriter.Write(jsonCambios.ToString());
            }
        }

        public virtual string customize(Dictionary<string, object> cambios, List<AudioContainer> audios, bool saveChange){
			// cambio el nombre
            if (cambios.ContainsKey("newName")){
				string newName = (string) cambios["newName"];
				this.changeNombre(newName);
			}

            //cambio el audio
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
                string jsonCambios = JsonConvert.SerializeObject(cambios);
                storeNewCustomization(jsonCambios);
            }
            return "";
        }
    }
}