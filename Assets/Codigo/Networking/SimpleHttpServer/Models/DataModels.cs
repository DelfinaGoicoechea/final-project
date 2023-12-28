using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SimpleHttpServer.Models
{
    [System.Serializable]
    public class DataPlay{
        public string scene;
        public static DataPlay CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<DataPlay>(jsonString);
        }
    }

    [System.Serializable]
    public class DataVolumen
    {
        public float volumen;
        public static DataVolumen CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<DataVolumen>(jsonString);
        }
    }

    [System.Serializable]
    public class DataMetricsList
    {
        public List<string> metrics;

        public DataMetricsList(List<string> metrics){
            this.metrics = metrics;
        }
        public string getAsJSON()
        {
            StringBuilder json = new StringBuilder("{\"metrics\": [");
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
