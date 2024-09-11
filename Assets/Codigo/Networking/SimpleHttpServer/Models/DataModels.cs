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
    public class ExternalAppInfo{
        public string visibleName;
        public string packageName;
        public static ExternalAppInfo CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<ExternalAppInfo>(jsonString);
        }

        public string CreateJSON()
        {
           return JsonUtility.ToJson(this);
        }
    }

    [System.Serializable]
    public class FilePathInfo{
        public string filepath;
        public static FilePathInfo CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<FilePathInfo>(jsonString);
        }

        public string CreateJSON()
        {
           return JsonUtility.ToJson(this);
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
