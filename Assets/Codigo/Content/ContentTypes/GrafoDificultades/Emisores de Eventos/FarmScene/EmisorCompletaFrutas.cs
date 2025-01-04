using UnityEngine;

public class EmisorCompletaFrutas : MonoBehaviour {
    public DificultadesFarmScene dificultades;
    public double fruitAmount;
    private double currentFruitCount = 0;

    public void reiniciarContador() {
        currentFruitCount = 0;
    }

    public void setFruitAmount(double number) {
        fruitAmount = number;
    }

    public void agregarFruta() {
        currentFruitCount++;
        if (currentFruitCount >= fruitAmount) {
            dificultades.procesarEvento("Completa");
        }
    }

    public void restarFruta() {
        currentFruitCount--;
    }

}