using System.Collections;
using UnityEngine;
using TMPro;

public class PedestrianTrafficLightController : MonoBehaviour
{
    public Light greenLight;  // Luz verde (persona caminando)
    public Light redLight;    // Luz roja (persona detenida)
    public TextMeshProUGUI contadorTexto;  // Texto en la UI para el contador

    public int greenDuration = 31;  // Tiempo en verde
    public int redDuration = 31;    // Tiempo en rojo

    private void Start()
    {
        StartCoroutine(TrafficLightCycle());
    }

    IEnumerator TrafficLightCycle()
    {
        while (true)
        {
            // Fase verde
            yield return StartCoroutine(ActualizarContador(greenDuration, Color.green));
            greenLight.enabled = false;
            redLight.enabled = true;

            // Fase roja
            yield return StartCoroutine(ActualizarContador(redDuration, Color.red));
            greenLight.enabled = true;
            redLight.enabled = false;
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
