using System.Collections;
using UnityEngine;
using TMPro;

public class PedestrianTrafficLightController : MonoBehaviour
{
    public Light greenLight;  
    public Light redLight;    
    public TextMeshProUGUI contadorTexto;  

    public int greenDuration = 31;  
    public int redDuration = 31;    

    private bool semaforoActivo = false;  

    private void Start()
    {
        // Apagar las luces al inicio
        greenLight.enabled = false;
        redLight.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!semaforoActivo && other.CompareTag("Player"))  // Asegura que solo se active una vez y por el jugador
        {
            semaforoActivo = true;
            redLight.enabled = true;
            greenLight.enabled = false;
            StartCoroutine(TrafficLightCycle());
        }
    }

    IEnumerator TrafficLightCycle()
    {
        while (true)
        {

            yield return StartCoroutine(ActualizarContador(redDuration, Color.red));
            greenLight.enabled = true;
            redLight.enabled = false;


            yield return StartCoroutine(ActualizarContador(greenDuration, Color.green));
            greenLight.enabled = false;
            redLight.enabled = true;

            
        }
    }

    IEnumerator ActualizarContador(int tiempo, Color color)
    {
        contadorTexto.color = color;

        for (int i = tiempo; i >= 0; i--)
        {
            contadorTexto.text = i.ToString();
            yield return new WaitForSeconds(1);
        }
    }
}
