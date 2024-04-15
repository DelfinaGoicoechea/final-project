using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SimpleHttpServer.Models
{
    [System.Serializable]
    /// <summary>
    /// Modelo de datos serializable para la escena a partir de json.
    /// </summary>
    public class DataScene{
        public string scene;
        public static DataScene CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<DataScene>(jsonString);
        }
    }

    [System.Serializable]
    public class DataSavedCustomizations{
        public string contentType;
        public string cambios;
        public static DataSavedCustomizations CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<DataSavedCustomizations>(jsonString);
        }
    }

    [System.Serializable]
    public class DataCustomizePhoto{
        public string oldName;
        public string newName;
        public string newAudio;
        public static DataCustomizePhoto CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<DataCustomizePhoto>(jsonString);
        }
    }

    [System.Serializable]
    public class DataCustomizeEscena{
        public string oldName;
        public string newName;
        public string newAudio;
        public static DataCustomizeEscena CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<DataCustomizeEscena>(jsonString);
        }
    }

    [System.Serializable]
    public class DataCustomizeVideo{
        public string oldName;
        public string newName;
        public string newAudio;
        public bool isEquiangular;
        public bool changedRepresentation;
        public static DataCustomizeVideo CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<DataCustomizeVideo>(jsonString);
        }
    }

    [System.Serializable]
    /// <summary>
    /// Modelo de datos serializable para el volumen a partir de json.
    /// </summary>
    public class DataVolumen
    {
        public float volumen;
        public static DataVolumen CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<DataVolumen>(jsonString);
        }
    }

    [System.Serializable]
    /// <summary>
    /// Modelo de datos serializable para la lista de metricas a partir de json.
    /// </summary>
    public class DataMetricsList
    {
        public List<string> metrics;

        public DataMetricsList(List<string> metrics){
            this.metrics = metrics;
        }
        public string getAsJSON()
        {
            StringBuilder json = new StringBuilder("{\"metrics\": [ ");
            foreach (var metric in metrics)
            {
                json.Append(metric +",");
            }
            json = json.Remove(json.Length-1, 1);
            json.Append("]}");
            return json.ToString();
        }
    }
}
