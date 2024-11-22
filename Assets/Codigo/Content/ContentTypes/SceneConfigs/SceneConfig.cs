using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class SceneConfig : MonoBehaviour {
    /// En esta variable se guardan las configuraciones validas para cada
    /// posible configuracion de la escena, Ejemplo:
    /// {
    ///    dificultad: ["baja", "media", "alta"],
    ///    duracion: [30, 60, 90],
    /// }
    /// Por lo que se debería asignar sus valores en cada clase que implementa SceneConfig
    public Dictionary<string, object> validConfigValues;

    public abstract void loadConfig(Dictionary<string, object> config);
}