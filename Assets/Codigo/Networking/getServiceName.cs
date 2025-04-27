using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleHttpServer;
using TMPro;

public class getServiceName : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        NetworkManager nm = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        if (nm.serviceName != null) {
            this.gameObject.GetComponent<TMP_Text>().text = nm.serviceName;
        }
    }
}
