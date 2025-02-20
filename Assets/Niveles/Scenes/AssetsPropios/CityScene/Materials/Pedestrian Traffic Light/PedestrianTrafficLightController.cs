using System.Collections;
using UnityEngine;
using TMPro;

public class PedestrianTrafficLightController : MonoBehaviour
{
    public Light greenLight;  // Luz verde
    public Light redLight;    // Luz roja
    public TextMeshProUGUI contadorTexto;  // Texto en la UI
    public AudioSource audioSource;  // Componente de sonido
    public AudioClip sonidoVerde;  // Sonido cuando el semáforo está en verde

    public MetricaTiempoReaccion metricaTiempoReaccion;

    public int greenDuration = 31;  // Tiempo en verde
    public int redDuration = 31;    // Tiempo en rojo

    private bool semaforoActivo = false;  

    private void Start()
    {
        // Apagar las luces y ocultar el contador al inicio
        greenLight.enabled = false;
        redLight.enabled = false;
        contadorTexto.text = "";  // No mostrar el contador al inicio
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!semaforoActivo && other.CompareTag("Player"))  
        {
            semaforoActivo = true;
            StartCoroutine(TrafficLightCycle());  
        }
    }

    IEnumerator TrafficLightCycle()
    {
        // 🔴 PRIMERA FASE: EMPEZAR EN ROJO  
        redLight.enabled = true;  
        greenLight.enabled = false;
        yield return StartCoroutine(ActualizarContador(redDuration, Color.red));

        // 🟢 FASE VERDE  
        redLight.enabled = false;  
        greenLight.enabled = true;

        // Reproducir sonido al cambiar a verde
        if (sonidoVerde != null && audioSource != null)
        {
            audioSource.PlayOneShot(sonidoVerde);
        }

        metricaTiempoReaccion.tiempoVerde();

        yield return StartCoroutine(ActualizarContador(greenDuration, Color.green));

        

        // 🔴 VOLVER A ROJO  
        StartCoroutine(TrafficLightCycle());
    }

    IEnumerator ActualizarContador(int tiempo, Color color)
    {
        contadorTexto.color = color;

        for (int i = tiempo; i > 0; i--)  // Ahora baja de 31 a 1 sin bug
        {
            contadorTexto.text = i.ToString();
            yield return new WaitForSeconds(1);
        }

        contadorTexto.text = "";  // Ocultar el contador cuando termine
    }


}
