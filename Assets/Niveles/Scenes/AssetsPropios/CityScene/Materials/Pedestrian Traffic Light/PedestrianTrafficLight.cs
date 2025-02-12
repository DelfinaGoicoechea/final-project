
using System.Collections;
using UnityEngine;

public class PedestrianTrafficLight : MonoBehaviour
{
    public Light greenLight;  // Luz verde (persona caminando)
    public Light redLight;    // Luz roja (persona detenida)

    public float greenDuration = 5f;  // Tiempo en verde
    public float redDuration = 5f;    // Tiempo en rojo

    private void Start()
    {
        StartCoroutine(TrafficLightCycle());
    }

    IEnumerator TrafficLightCycle()
    {
        while (true)
        {
            // Activa la luz verde y apaga la roja
            greenLight.enabled = true;
            redLight.enabled = false;
            yield return new WaitForSeconds(greenDuration);

            // Activa la luz roja y apaga la verde
            greenLight.enabled = false;
            redLight.enabled = true;
            yield return new WaitForSeconds(redDuration);
        }
    }
}
