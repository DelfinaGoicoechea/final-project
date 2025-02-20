using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using SimpleHttpServer.Models;
using Newtonsoft.Json.Linq;

namespace Contents{

    /// <summary>
    /// Este tipo de contenido (ExternalApp) solo sirve para que el listado de apps lanzables
    /// dependa del propio Oculus, y no del cliente en Flutter.
    /// No hace nada más, ya que los comandos para iniciar la propia "ExternalApp" se envian
    /// desde una pc utilizando adb, y el packageName obtenido de aca.
    /// </summary>
    public class ExternalApp: Content
    {
       [Tooltip("Nombre del package a lanzar")]
        public string packageName;
        // Start is called before the first frame update

        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }
        
        public override void play(Dictionary<string, object> config)
        {
        }

        public override void pause()
        {
        }
        
        public override void stop()
        {
            base.stop();
        }

        /// <summary>
        /// Agrega el nombre del contenido a un Diccionario que
        ///  contiene listas de nombres de contenidos, bajo la key
        ///  "3D".
        /// </summary>
        /// <param name="dictio"></param>
        public override void addToDictionary(Dictionary<string, List<string>> dictio){
            if (!dictio.ContainsKey("App")){
                dictio.Add("App", new List<string>());
            }
            dictio["App"].Add(this.nombre);
        }

        public override bool delete() {
            if (thumbnail != null && thumbnail != Texture2D.whiteTexture) {
                UnityEngine.Object.Destroy(thumbnail);
            }
            string extAppsFileLocation = $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}externalApps.txt";
			if(File.Exists(extAppsFileLocation)){
                string[] extAppInfos = File.ReadAllLines(extAppsFileLocation);
                StreamWriter streamWriter = File.CreateText(extAppsFileLocation);
                if(extAppInfos != null){
                    // Escribo todas las lineas excepto la que corresponde a esta ExternalApp
                    foreach (string info in extAppInfos) {
                        if(info != null){
                            ExternalAppInfo extAppInfo = ExternalAppInfo.CreateFromJSON(info);
                            if(extAppInfo != null){
                                if (extAppInfo.visibleName != null && extAppInfo.visibleName != this.nombre) {
                                    streamWriter.WriteLine(info);
                                }
                            }
                        }

                    }
                }
                streamWriter.Close();
                return base.delete();
            }
            return false;
        }

        public override Dictionary<string,object> getCurrentConfig() {
            Dictionary<string,object> config = base.getCurrentConfig();
            config["packageName"] = this.packageName;
            return config;
        }

        // En el caso de que se desee tener distintos Thumbnails por cada App,
        // va a ser necesario un override de changeName(), para que al cambiar el nombre
        // del ExternalApp, tambien se cambie el nombre del archivo de thumbnail correspondiente.

        public override bool changeNombre(string newName){
            // Util para meter los cambios en el propio listado de ExternalApps
            string extAppsFileLocation = $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}externalApps.txt";
			string[] stringAppInfos = File.ReadAllLines(extAppsFileLocation);
            StreamWriter streamWriter = File.CreateText(extAppsFileLocation); // Borro el archivo para rellenarlo con info actualizada
            foreach (string stringAppInfo in stringAppInfos){
                ExternalAppInfo extAppInfo = ExternalAppInfo.CreateFromJSON(stringAppInfo);
                if  (this.nombre == extAppInfo.visibleName) {
                    // escribo con nombre cambiado
                    extAppInfo.visibleName = newName;
                }
                streamWriter.WriteLine(extAppInfo.CreateJSON());
            }
            streamWriter.Close();
            this.nombre = newName;
            return true;
        }

        protected bool changePackageName(string newPackageName){
            // Util para meter los cambios en el propio listado de ExternalApps
            string extAppsFileLocation = $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}externalApps.txt";
			string[] stringAppInfos = File.ReadAllLines(extAppsFileLocation);
            StreamWriter streamWriter = File.CreateText(extAppsFileLocation); // Borro el archivo para rellenarlo con info actualizada
            foreach (string stringAppInfo in stringAppInfos){
                ExternalAppInfo extAppInfo = ExternalAppInfo.CreateFromJSON(stringAppInfo);
                if  (this.packageName == extAppInfo.packageName) {
                    // escribo con nombre cambiado
                    extAppInfo.packageName = newPackageName;
                }
                streamWriter.WriteLine(extAppInfo.CreateJSON());
            }
            streamWriter.Close();
            this.packageName = newPackageName;
            return true;
        }

        public override string customize(Dictionary<string, object> cambios, List<AudioContainer> audios, bool saveChange = true){
            bool hasNewName = cambios.ContainsKey("newName");
            if (hasNewName){
				string newName = (string) cambios["newName"];
				this.changeNombre(newName); // cambia el nombre y almacena el cambio
			}

            if (hasNewName){
                // Elimino del dict, xq el cambio ya se almacena
                cambios.Remove("newName");
            }

			bool hasPackageName = cambios.ContainsKey("packageName");
			if (hasPackageName){
				string packageName = (string) cambios["packageName"];
				if(packageName == ""){
					string warning = $"El packagenName de {this.nombre} no puede estar vacio";
					Debug.LogWarning(warning);
					return warning;
				}else{
					this.changePackageName(packageName);
                    cambios.Remove("packageName"); // Elimino del dict, xq el cambio ya se almacena
				}
			}

            if (hasNewName || hasPackageName){
                // Guardo la fecha del cambio
                string dateFileLocation = $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}lastChangeDate.txt";
                string dateToStore = DateTime.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture);
                if(!File.Exists(dateFileLocation)){
                    File.Create(dateFileLocation).Dispose();
                }
                File.WriteAllText(dateFileLocation, dateToStore);
            }
			
			// Los cambios ya se guardan en changeNombre(), changePackageName()
            return ""; 
        }
    }
}