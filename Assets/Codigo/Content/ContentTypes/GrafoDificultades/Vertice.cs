#nullable enable
using System.Collections;
using System.Collections.Generic;
public class Vertice {
    private string nombreVertice;
    private List<Arco>? arcosAdyacencias;

    public Vertice(string nombreVertice, List<Arco>? arcosAdyacencias){
        this.nombreVertice = nombreVertice;
        if (arcosAdyacencias != null){
            this.arcosAdyacencias = arcosAdyacencias;
        }
    }

    public Arco? getAdyacente(string evento){
        if (arcosAdyacencias == null) return null;
        return arcosAdyacencias.Find(arco => arco.evento == evento);
    }

    public string getNombreVertice(){
        return nombreVertice;
    }
}