using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System;

namespace Contents{

    /// <summary>
    /// Definicion del tipo de contenido escena 3D
    /// </summary>
    public class Escena: Content
    {
       [Tooltip("Nombre de la escena a cargar (solo util y visible para Unity)")]
        public string sceneName;
        // Start is called before the first frame update
        public SceneConfig sceneConfig;
        void Start()
        {
            //or else
        }

        // Update is called once per frame
        void Update()
        {

        }
        
        private void OnLoadScene(AsyncOperation obj, Dictionary<string, object> currentConfig){
            if (sceneConfig != null && currentConfig != null) {
                sceneConfig.loadConfig(currentConfig);
            }
        }
        public override void play(Dictionary<string, object> config)
        {
            Time.timeScale = 1.0f;
            if (!isPaused) {
                AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName);
                asyncOp.completed += (asyncOp) => OnLoadScene(asyncOp, config);
            }
            base.play(config);
        }

        public override void pause()
        {
            base.pause();
            isPaused = true;
            Time.timeScale = 0.0f;
        }
        
        public override void stop()
        {
            base.stop();
            Time.timeScale = 1.0f;
        }

        /// <summary>
        /// Agrega el nombre del contenido a un Diccionario que
        ///  contiene listas de nombres de contenidos, bajo la key
        ///  "3D".
        /// </summary>
        /// <param name="dictio"></param>
        public override void addToDictionary(Dictionary<string, List<string>> dictio){
            if (!dictio.ContainsKey("3D")){
                dictio.Add("3D", new List<string>());
            }
            dictio["3D"].Add(this.nombre);
        }

        public override bool delete() {
            if (thumbnail != null && thumbnail != Texture2D.whiteTexture) {
                Resources.UnloadAsset(thumbnail);
            }
            string disablesFileLocation = $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}disables.txt";
            if (!File.Exists(disablesFileLocation)) {
                try {
                    File.Create(disablesFileLocation).Dispose();
                    using (StreamWriter streamWriter = File.AppendText(disablesFileLocation)) {
                        streamWriter.Write(this.nombre);
                    }
                    return true;
                } catch (Exception e) {
                    Debug.Log(e.StackTrace);
                    return false;
                }
            } else {
                // si el archivo existe, pero esta vacio, no meto salto de linea al inicio
                long fileLength = new FileInfo(disablesFileLocation).Length;
                string toWrite = (fileLength == 0) ? this.nombre : "\n"+this.nombre;
                try {
                    using (StreamWriter streamWriter = File.AppendText(disablesFileLocation)) {
                        streamWriter.Write(toWrite);
			        }
                    return base.delete();
                } catch (Exception e) {
                    Debug.Log(e.StackTrace);
                    return false;
                }
            }
        }

        public override string customize(Dictionary<string, object> cambios, List<AudioContainer> audios, bool saveChange){
            bool hasNewName = cambios.ContainsKey("newName");
            bool hasNewAudio = cambios.ContainsKey("newAudio");
            bool hasTags = cambios.ContainsKey("tags");
            bool shouldSave = saveChange && (hasNewName || hasNewAudio || hasTags);
            return base.customize(cambios, audios, shouldSave);    
        }
    }
}