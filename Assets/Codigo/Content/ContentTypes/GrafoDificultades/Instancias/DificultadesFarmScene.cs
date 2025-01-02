using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DificultadesFarmScene : MonoBehaviour {

    private GrafoDificultades grafoDificultades;
    
    // Todos estos campos los asigna FarmSceneConfig al cargarse la escena
    public Vector3[] lugaresSpawn;
    public long nroTomates;
    public long nroPeras;
    public GameObject tomatePrefab;
    public GameObject tomateShadowPrefab;
    public GameObject peraPrefab;
    public GameObject peraShadowPrefab;
    public bool usingShadows;

    public bool procesarEvento(string evento) {
        bool result = grafoDificultades.procesarEvento(evento);
        Debug.Log($"{evento}: {result}");
        return result;
    }

    // Definir las funciones que va a usar c/ arco
    private void achicarFrutas() {
        GameObject[] frutas = GameObject.FindGameObjectsWithTag("Fruta");
        foreach (GameObject fruta in frutas) {
            fruta.transform.localScale = fruta.transform.localScale * 0.5f;
        }
    }

    private void agrandarFrutas() {
        GameObject[] frutas = GameObject.FindGameObjectsWithTag("Fruta");
        foreach (GameObject fruta in frutas) {
            fruta.transform.localScale = fruta.transform.localScale * 2.0f;
        }
    }
    
    private void quitarColores() {
        usingShadows = true;
        // Iba a cambiar el color de las ya existentes, pero es mas facil definir
        // usingShadows y usar respawn.
        
        /* GameObject spawnPoint = GameObject.FindWithTag("SpawnPoint");
        GameObject[] frutas = GameObject.FindGameObjectsWithTag("Fruta");
        foreach (GameObject fruta in frutas) {
            Vector3 position;
            Quaternion rotation;
            Vector3 localScale;
            fruta.transform.GetPositionAndRotation(position, rotation);
            localScale = fruta.transform.localScale;
            // GameObject frutaNew = Instantiate(tomate_a_instanciar, spawnPoint.transform);
            // No se que  fruta Instanciar. Implementar cambiando el material.
            // Puedo averiguar el prefab?
        } */
    }

    private void devolverColores() {
        usingShadows = false;
    }

    private void respawnFrutas() {
        // Elimino todas las frutas que ya existen en la escena
        GameObject[] frutas = GameObject.FindGameObjectsWithTag("Fruta");
        foreach (GameObject fruta2Delete in frutas) {
            UnityEngine.Object.Destroy(fruta2Delete);
        }

        GameObject spawnPoint = GameObject.FindWithTag("SpawnPoint");
        List<int> availableSpawns = new List<int>(Enumerable.Range(0, lugaresSpawn.Length));
        System.Random rand = new System.Random();
        GameObject tomate_a_instanciar = (usingShadows) ? tomateShadowPrefab : tomatePrefab;
        if (tomate_a_instanciar != null) {
            for (int i = 0; i < nroTomates; i++) {
                int spawnIndex = rand.Next(0, availableSpawns.Count);
                GameObject apple = Instantiate(tomate_a_instanciar, spawnPoint.transform);
                apple.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
                apple.transform.localPosition = lugaresSpawn[availableSpawns[spawnIndex]];
                availableSpawns.RemoveAt(spawnIndex);
            }
        }

        GameObject pera_a_instanciar = (usingShadows) ? peraShadowPrefab : peraPrefab;
        if (pera_a_instanciar != null) {
            for (int i = 0; i < nroPeras; i++) {
                int spawnIndex = rand.Next(0, availableSpawns.Count);
                GameObject pear = Instantiate(pera_a_instanciar, spawnPoint.transform);
                pear.transform.localPosition = lugaresSpawn[availableSpawns[spawnIndex]];
                availableSpawns.RemoveAt(spawnIndex);
            }
        }
    }

    private void modificarNroFrutas(int nro) {
        // me hace falta ver como manejar el nro de frutas actual
        // ¿Variable en esta clase y que sea tomada por closure?
        // tengo que ver si el nro total es valido
        // (deberia asignar mas ubicaciones que anden bien)
    }

    private void mostrarFelicitacion() {
        return;
    }

    void Awake() {
        // crear grafo (vertices) y asignar arcos
        grafoDificultades = new GrafoDificultades(6); // 6 vertices
        List<Arco> arcosEstadoCero = new List<Arco> {
            new Arco("Completa", 1, new List<Arco.VoidMethodDelegate>{ respawnFrutas, achicarFrutas })
        };
        List<Arco> arcosEstadoUno = new List<Arco> {
            new Arco("Completa", 2, new List<Arco.VoidMethodDelegate>{ quitarColores, respawnFrutas }),
            new Arco("Falla", 0, new List<Arco.VoidMethodDelegate>{ agrandarFrutas }),
        };
        List<Arco> arcosEstadoDos = new List<Arco> {
            new Arco("Completa", 4, new List<Arco.VoidMethodDelegate>{ respawnFrutas, achicarFrutas }),
            new Arco("Falla", 3, new List<Arco.VoidMethodDelegate>{ devolverColores, respawnFrutas }),
        };
        List<Arco> arcosEstadoTres = new List<Arco> {
            new Arco("Completa", 2, new List<Arco.VoidMethodDelegate>{ quitarColores, respawnFrutas }),
        };
        List<Arco> arcosEstadoCuatro = new List<Arco> {
            // Acá irían las felicitaciones
            //new Arco("Completa", 2, new List<Arco.VoidMethodDelegate>()),
        };
        grafoDificultades.asignarArcos(0, arcosEstadoCero);
        grafoDificultades.asignarArcos(1, arcosEstadoUno);
        grafoDificultades.asignarArcos(2, arcosEstadoDos);
        grafoDificultades.asignarArcos(3, arcosEstadoTres);
        grafoDificultades.asignarArcos(4, arcosEstadoCuatro);
    }
}