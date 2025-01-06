#nullable enable
using System.Collections.Generic;

public class GrafoDificultades {
    private int estadoActual = 0;
    private int size;
    private List<Arco>[] vertices;

    // constructor
    public GrafoDificultades(int size) {
        this.size = size;
        vertices = new List<Arco>[size];
    }

    public bool asignarArcos(int vertice, List<Arco> arcos) {
        if (vertice < 0 || vertice >= size) {
            return false;
        }
        vertices[vertice] = arcos;
        return true;
    }

    // procesar evento
    public bool procesarEvento(string evento) {
        Arco? arcoMatch = vertices[estadoActual].Find(arco => arco.evento == evento);
        if (arcoMatch == null) return false; // No hay arco
        if (arcoMatch.proxEstado < 0 || arcoMatch.proxEstado >= size) return false; // proximo estado fuera de limites
        if(arcoMatch.acciones != null){
            foreach(Arco.VoidMethodDelegate accion in arcoMatch.acciones){
                accion.Invoke();
            }
        }
        this.estadoActual = arcoMatch.proxEstado;
        return true;
    }    
}