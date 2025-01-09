#nullable enable
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DificultadesFarmScene : MonoBehaviour {

    private GrafoDificultades? grafoDificultades;
    [SerializeField] private AudioSource? fxAudioSource;
    [SerializeField] private AudioClip? sonidoFinalizacion;
    [SerializeField] private GameObject? cartelFinalizacion;
    [SerializeField] private MetricaSoltarFrutas? metricaSoltarFrutas;
    
    // NOTA IMPORTANTE: 
    //  Todos los siguientes campos son private para que sea evidente en el Editor de 
    //  Unity cuales son los atributos que se deben asignar alli y cuales no, pero todos
    //  estos campos (private) los asigna FarmSceneConfig al cargarse la escena.
    private Vector3[]? lugaresSpawn;
    private long nroTomates;
    private long nroPeras;
    private GameObject? tomatePrefab;
    private GameObject? tomateShadowPrefab;
    private GameObject? peraPrefab;
    private GameObject? peraShadowPrefab;
    private bool usingShadows;
    
    public void setLugaresSpawn(Vector3[] lugares) => lugaresSpawn = lugares;
    public void setNroTomates(long nro) => nroTomates = nro;
    public void setNroPeras(long nro) => nroPeras = nro;
    public void setTomatePrefab(GameObject prefab) => tomatePrefab = prefab;
    public void setTomateShadowPrefab(GameObject prefab) => tomateShadowPrefab = prefab;
    public void setPeraPrefab(GameObject prefab) => peraPrefab = prefab;
    public void setPeraShadowPrefab(GameObject prefab) => peraShadowPrefab = prefab;
    public void setUsingShadows(bool use) => usingShadows = use;

    public string? procesarEvento(string evento) {
        string? nombreNuevoVertice = grafoDificultades?.procesarEvento(evento);
        if(nombreNuevoVertice != null && metricaSoltarFrutas != null) {
            metricaSoltarFrutas.cambiaDificultad(nombreNuevoVertice);
            // Si se necesitara, se puede generalizar usando una lista de funciones a ejecutar,
            // similar a lo que se hace en procesarEvento() de GrafoDificultades.
        }
        return nombreNuevoVertice;
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

        // Spawn nuevas frutas
        GameObject spawnPoint = GameObject.FindWithTag("SpawnPoint");
        List<int> availableSpawns = new(Enumerable.Range(0, lugaresSpawn!.Length));
        System.Random rand = new();
        GameObject? tomate_a_instanciar = (usingShadows) ? tomateShadowPrefab : tomatePrefab;
        if (tomate_a_instanciar != null) {
            for (int i = 0; i < nroTomates; i++) {
                int spawnIndex = rand.Next(0, availableSpawns.Count);
                GameObject apple = Instantiate(tomate_a_instanciar, spawnPoint.transform);
                apple.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
                apple.transform.localPosition = lugaresSpawn[availableSpawns[spawnIndex]];
                availableSpawns.RemoveAt(spawnIndex);
            }
        }

        GameObject? pera_a_instanciar = (usingShadows) ? peraShadowPrefab : peraPrefab;
        if (pera_a_instanciar != null) {
            for (int i = 0; i < nroPeras; i++) {
                int spawnIndex = rand.Next(0, availableSpawns.Count);
                GameObject pear = Instantiate(pera_a_instanciar, spawnPoint.transform);
                pear.transform.localPosition = lugaresSpawn[availableSpawns[spawnIndex]];
                availableSpawns.RemoveAt(spawnIndex);
            }
        }

        // reinicio el EmisorCompletaFrutas (en el mismo GameObject que este script)
        EmisorEventosFrutas emisor = gameObject.GetComponent<EmisorEventosFrutas>();
        if (emisor != null) {
            emisor.reiniciarContador();
            emisor.setFruitAmount(nroTomates + nroPeras);
        }
    }

    private void modificarNroFrutas(int nro) {
        // me hace falta ver como manejar el nro de frutas actual
        // ¿Variable en esta clase y que sea tomada por closure?
        // tengo que ver si el nro total es valido
        // (deberia asignar mas ubicaciones que anden bien)
    }

    private void finalizacionExitosa() {
        // Elimino todas las frutas que ya existen en la escena
        GameObject[] frutas = GameObject.FindGameObjectsWithTag("Fruta");
        foreach (GameObject fruta2Delete in frutas) {
            UnityEngine.Object.Destroy(fruta2Delete);
        }
        // Reproduzco sonido y muestro cartel de finalización
        StartCoroutine(sonidoTextoFinalizacion());
    }

    IEnumerator sonidoTextoFinalizacion(){
        if (fxAudioSource != null && fxAudioSource.isPlaying) {
            yield return new WaitForSeconds(fxAudioSource.clip.length);
        }
        fxAudioSource!.clip = sonidoFinalizacion;
        fxAudioSource.Play();
        cartelFinalizacion?.SetActive(true);
    }


    void Awake() {
        // crear grafo (vertices) y asignar arcos
        grafoDificultades = new GrafoDificultades(6); // 6 vertices
        List<Arco> arcosEstadoCero = new List<Arco> {
            // Pasa a coloreadas pequeñas
            new Arco("Completa", 1, new List<Arco.VoidMethodDelegate>{ respawnFrutas, achicarFrutas })
        };
        List<Arco> arcosEstadoUno = new List<Arco> {
            // pasa a sombras grandes
            new Arco("Completa", 2, new List<Arco.VoidMethodDelegate>{ quitarColores, respawnFrutas }),
            // vuelve a coloreadas grandes
            new Arco("Falla", 0, new List<Arco.VoidMethodDelegate>{ respawnFrutas }), // Tambien devuelve el tamaño original
        };
        List<Arco> arcosEstadoDos = new List<Arco> {
            // pasa a sombras pequeñas
            new Arco("Completa", 4, new List<Arco.VoidMethodDelegate>{ respawnFrutas, achicarFrutas }),
            // devuelve el color a las frutas (grandes)
            new Arco("Falla", 3, new List<Arco.VoidMethodDelegate>{ devolverColores, respawnFrutas }),
        };
        List<Arco> arcosEstadoTres = new List<Arco> {
            // vuelve a sombras grandes
            new Arco("Completa", 2, new List<Arco.VoidMethodDelegate>{ quitarColores, respawnFrutas }),
        };
        List<Arco> arcosEstadoCuatro = new List<Arco> {
            // finaliza el ejercicio
            new Arco("Completa", 5, new List<Arco.VoidMethodDelegate>(){ finalizacionExitosa }),
        };

        grafoDificultades.asignarVertice(0, new Vertice("Coloreados grandes", arcosEstadoCero));
        grafoDificultades.asignarVertice(1, new Vertice("Coloreados chicos", arcosEstadoUno));
        grafoDificultades.asignarVertice(2, new Vertice("Sombras grandes", arcosEstadoDos));
        grafoDificultades.asignarVertice(3, new Vertice("Colores grandes revelados", arcosEstadoTres));
        grafoDificultades.asignarVertice(4, new Vertice("Sombras chicas", arcosEstadoCuatro));
        grafoDificultades.asignarVertice(5, new Vertice("Finalizado", null));
        
    }

    void Start() {
        // Informo el nombre del estado inicial
        if (metricaSoltarFrutas != null) {
            metricaSoltarFrutas.cambiaDificultad("Coloreados grandes");
        }
    }
}