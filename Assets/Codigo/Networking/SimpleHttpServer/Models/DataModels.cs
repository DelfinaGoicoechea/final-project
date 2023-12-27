using UnityEngine;
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
}
