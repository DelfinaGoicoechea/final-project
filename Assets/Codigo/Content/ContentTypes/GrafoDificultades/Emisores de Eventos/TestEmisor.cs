using UnityEngine;

public class TestEmisor : MonoBehaviour {

    public bool completado;
    private bool sentCompletado;
    public bool fallo;
    private bool sentFallo;
    private DificultadesFarmScene dificultades;

    void Start() {
        dificultades = GameObject.FindWithTag("GrafoDificultad").GetComponent<DificultadesFarmScene>();
    }

    void Update(){
        if (completado && !sentCompletado) {
            dificultades.procesarEvento("Completa");
            sentCompletado = true;
        } else {
            if (!completado) {
                sentCompletado = false;
            }
        }

        if (fallo && !sentFallo) {
            dificultades.procesarEvento("Falla");
            sentFallo = true;
        } else {
            if (!fallo) {
                sentFallo = false;
            }
        }
    }
}