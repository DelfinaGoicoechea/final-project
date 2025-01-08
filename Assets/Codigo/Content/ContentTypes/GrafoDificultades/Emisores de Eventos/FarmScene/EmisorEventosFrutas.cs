using UnityEngine;

public class EmisorEventosFrutas : MonoBehaviour {
    public DificultadesFarmScene dificultades;
    public double fruitAmount;
    private double currentFruitCount = 0;
    private double maxErrorAmount;
    private double currentErrorCount = 0;

    public void reiniciarContador() {
        currentFruitCount = 0;
        currentErrorCount = 0;
    }

    public void setFruitAmount(double number) {
        fruitAmount = number;
        maxErrorAmount = System.Math.Floor(number * 0.5);
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
    public void agregarCajonErroneo() {
        currentErrorCount++;
        if (currentErrorCount >= maxErrorAmount) {
            dificultades.procesarEvento("Falla");
        }
    }
}