using UnityEngine;
using TMPro;
using System.Collections;

public class Contador : MonoBehaviour
{
    public TextMeshProUGUI contadorTexto;  // Referencia al texto en la UI
    private int tiempo = 30;  // Tiempo inicial
    private bool esRojo = false;  // Variable para alternar colores

    void Start()
    {
        StartCoroutine(ContadorTiempo());
    }

    IEnumerator ContadorTiempo()
    {
        while (true)  // Loop infinito
        {
            contadorTexto.color = esRojo ? Color.red : Color.green;  // Alternar color
            
            while (tiempo >= 0)  
            {
                contadorTexto.text = tiempo.ToString();  // Mostrar el tiempo en pantalla
                yield return new WaitForSeconds(1);  // Esperar 1 segundo
                tiempo--;  // Restar 1 al contador
            }

            esRojo = !esRojo;  // Alternar entre rojo y verde
            tiempo = 30;  // Reiniciar contador
        }
    }
}
