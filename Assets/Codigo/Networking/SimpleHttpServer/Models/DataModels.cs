using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace SimpleHttpServer.Models
{


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
            return JsonUtility.ToJson(this);
        }
    }
}
