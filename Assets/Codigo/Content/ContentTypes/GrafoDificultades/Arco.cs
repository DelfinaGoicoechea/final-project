using System.Collections;
using System;
using System.Collections.Generic;

public class Arco {
    public delegate void VoidMethodDelegate();
    public string evento;
    public int proxEstado;
    public List<VoidMethodDelegate>? acciones; // Listado de cambios en la escena
    public Arco(string evento, int proxEstado, List<VoidMethodDelegate>? acciones) {
        this.evento = evento;
        this.proxEstado = proxEstado;
        this.acciones = acciones;
    }
}