using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FarmSceneConfig : SceneConfig {
    public GameObject tomatePrefab;
    public GameObject tomateShadowPrefab;
    public GameObject peraPrefab;
    public GameObject peraShadowPrefab;
    private static Vector3[] lugaresSpawn = new Vector3[8] { 
        new Vector3(-0.156f, 0.1f, -0.107f), new Vector3(0.162f, 0.1f, 0.005f),
        new Vector3(0.165f, 0.1f, 0.106f), new Vector3(0.026f, 0.1f, -0.094f),
        new Vector3(0.035f, 0.1f, 0.094f), new Vector3(-0.085f, 0.1f, -0.111f),
        new Vector3(-0.121f, 0.1f, 0.006f), new Vector3(-0.110f, 0.1f, 0.101f)
    };

    public FarmSceneConfig(){
        validConfigValues = new Dictionary<string, object>();
        int[] nroTomates = new int[3] { 2, 3, 4 };
        int[] nroPeras = new int[3] { 2, 3, 4 };
        bool[] useShadows = new bool[2] { true, false };
        validConfigValues.Add("nroTomates", nroTomates);
        validConfigValues.Add("nroPeras", nroPeras);
        validConfigValues.Add("useShadows", useShadows);
    }

    public override void loadConfig(Dictionary<string, object> config) {
        GameObject spawnPoint = GameObject.FindWithTag("SpawnPoint");
        long nroTomates = 4;
        long nroPeras = 4;
        bool useShadows = false;
        if (config.ContainsKey("nroTomates")) {
            nroTomates = (long) config["nroTomates"];
        }
        if (config.ContainsKey("nroPeras")) {
            nroPeras = (long) config["nroPeras"];
        }
        if (config.ContainsKey("useShadows")) {
            useShadows = (bool) config["useShadows"];
        }

        List<int> availableSpawns = new List<int> {0, 1, 2, 3, 4, 5, 6, 7};
        System.Random rand = new System.Random();
        GameObject tomate_a_instanciar = (useShadows) ? tomateShadowPrefab : tomatePrefab;
        if (tomate_a_instanciar != null) {
            for (int i = 0; i < nroTomates; i++) {
                int spawnIndex = rand.Next(0, availableSpawns.Count);
                GameObject apple = Instantiate(tomate_a_instanciar, spawnPoint.transform);
                apple.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
                apple.transform.localPosition = lugaresSpawn[availableSpawns[spawnIndex]];
                availableSpawns.RemoveAt(spawnIndex);
            }
        }

        GameObject pera_a_instanciar = (useShadows) ? peraShadowPrefab : peraPrefab;
        if (pera_a_instanciar != null) {
            for (int i = 0; i < nroPeras; i++) {
                int spawnIndex = rand.Next(0, availableSpawns.Count);
                GameObject pear = Instantiate(pera_a_instanciar, spawnPoint.transform);
                pear.transform.localPosition = lugaresSpawn[availableSpawns[spawnIndex]];
                availableSpawns.RemoveAt(spawnIndex);
            }
        }

        // Asigno atributos al grafo de dificultades, para que ahora maneje las cosas el.
        GameObject grafoGameObject = GameObject.FindWithTag("GrafoDificultad");
        if (grafoGameObject != null) {
            // Asigno atributos al grafo
            DificultadesFarmScene grafoDific = grafoGameObject.GetComponent<DificultadesFarmScene>();
            if (grafoDific != null) {
                grafoDific.lugaresSpawn = lugaresSpawn;
                grafoDific.nroTomates = nroTomates;
                grafoDific.nroPeras = nroPeras;
                grafoDific.tomatePrefab = tomatePrefab;
                grafoDific.tomateShadowPrefab = tomateShadowPrefab;
                grafoDific.peraPrefab = peraPrefab;
                grafoDific.peraShadowPrefab = peraShadowPrefab;
                grafoDific.usingShadows = useShadows;
            }

            // Asigno atributos a los emisores
            EmisorCompletaFrutas emisorCompletaFrutas = grafoGameObject.GetComponent<EmisorCompletaFrutas>();
            if (emisorCompletaFrutas != null) {
                emisorCompletaFrutas.setFruitAmount(nroTomates + nroPeras);
            }
        }
    }
}