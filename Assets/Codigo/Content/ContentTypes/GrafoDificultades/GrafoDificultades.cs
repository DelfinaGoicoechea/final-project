#nullable enable
using System.Collections.Generic;

public class GrafoDificultades {
    private int estadoActual = 0;
    private int size;
    private Vertice[] vertices;

    // constructor
    public GrafoDificultades(int size) {
        this.size = size;
        vertices = new Vertice[size];
    }

    public bool asignarVertice(int index, Vertice vertice) {
        if (index < 0 || index >= size) {
            return false;
        }
        vertices[index] = vertice;
        return true;
    }

    /// <summary>
    /// procesar evento
    /// </summary>
    /// <param name="evento"></param>
    /// <returns> string (nombre) asociado al nuevo vertice (nivel)</returns>
    public string? procesarEvento(string evento) {
        Arco? arcoMatch = vertices[estadoActual].getAdyacente(evento);
        if (arcoMatch == null) return null; // No hay arco
        if (arcoMatch.proxEstado < 0 || arcoMatch.proxEstado >= size) return null; // proximo estado fuera de limites
        if(arcoMatch.acciones != null){
            foreach(Arco.VoidMethodDelegate accion in arcoMatch.acciones){
                accion.Invoke();
            }
        }
        this.estadoActual = arcoMatch.proxEstado;
        return vertices[estadoActual].getNombreVertice();
    }    
}