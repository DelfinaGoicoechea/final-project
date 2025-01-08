using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mono.Zeroconf.Providers.Bonjour;

public class DificultadesFarmScene : MonoBehaviour {

    private GrafoDificultades grafoDificultades;
    [SerializeField] private AudioSource fxAudioSource;
    [SerializeField] private AudioClip sonidoFinalizacion;
    [SerializeField] private GameObject cartelFinalizacion;
    
    // NOTA IMPORTANTE: 
    //  Todos estos campos son private para que sea evidente en el Editor de Unity
    //  cuales son los atributos que se deben asignar alli y cuales no, pero todos
    //  estos campos (private) los asigna FarmSceneConfig al cargarse la escena.
    private Vector3[] lugaresSpawn;
    private long nroTomates;
    private long nroPeras;
    private GameObject tomatePrefab;
    private GameObject tomateShadowPrefab;
    private GameObject peraPrefab;
    private GameObject peraShadowPrefab;
    private bool usingShadows;
    
    public void setLugaresSpawn(Vector3[] lugares) => lugaresSpawn = lugares;
    public void setNroTomates(long nro) => nroTomates = nro;
    public void setNroPeras(long nro) => nroPeras = nro;
    public void setTomatePrefab(GameObject prefab) => tomatePrefab = prefab;
    public void setTomateShadowPrefab(GameObject prefab) => tomateShadowPrefab = prefab;
    public void setPeraPrefab(GameObject prefab) => peraPrefab = prefab;
    public void setPeraShadowPrefab(GameObject prefab) => peraShadowPrefab = prefab;
    public void setUsingShadows(bool use) => usingShadows = use;

    public bool procesarEvento(string evento) {
        bool result = grafoDificultades.procesarEvento(evento);
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
        fxAudioSource.clip = sonidoFinalizacion;
        fxAudioSource.Play();
        cartelFinalizacion.SetActive(true);
    }


    void Awake() {
        // crear grafo (vertices) y asignar arcos
        grafoDificultades = new GrafoDificultades(6); // 6 vertices
        List<Arco> arcosEstadoCero = new List<Arco> {
            new Arco("Completa", 1, new List<Arco.VoidMethodDelegate>{ respawnFrutas, achicarFrutas })
        };
        List<Arco> arcosEstadoUno = new List<Arco> {
            new Arco("Completa", 2, new List<Arco.VoidMethodDelegate>{ quitarColores, respawnFrutas }),
            new Arco("Falla", 0, new List<Arco.VoidMethodDelegate>{ respawnFrutas }), // Tambien devuelve el tamaño original
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
            new Arco("Completa", 5, new List<Arco.VoidMethodDelegate>(){ finalizacionExitosa }),
        };
        //
        grafoDificultades.asignarArcos(0, arcosEstadoCero);
        grafoDificultades.asignarArcos(1, arcosEstadoUno);
        grafoDificultades.asignarArcos(2, arcosEstadoDos);
        grafoDificultades.asignarArcos(3, arcosEstadoTres);
        grafoDificultades.asignarArcos(4, arcosEstadoCuatro);
        grafoDificultades.asignarArcos(5, null);
    }
}