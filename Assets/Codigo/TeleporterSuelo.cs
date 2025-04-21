using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterSuelo : MonoBehaviour
{
    [SerializeField]
    private Transform spawnPoint; // Punto de aparicion asignado en el Editor de Unity

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Fruta"))
        {
            // Teletransportar la fruta al cajon
            collision.transform.position = spawnPoint.position + Vector3.up * 0.2f ;
        }
    }
}
