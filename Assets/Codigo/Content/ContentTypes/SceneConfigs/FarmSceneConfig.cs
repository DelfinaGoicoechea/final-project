using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FarmSceneConfig : SceneConfig {
    public GameObject tomatePrefab;
    public GameObject peraPrefab;
    private static Vector3[] lugaresSpawn = new Vector3[8] { 
        new Vector3(-0.030f, 0.1f, 0.008f), new Vector3(-0.055f, 0.1f, 0.411f),
        new Vector3(-0.029f, 0.1f, 0.758f), new Vector3(-0.345f, 0.1f, 0.050f),
        new Vector3(-0.337f, 0.1f, 0.465f), new Vector3(-0.326f, 0.1f, 0.746f),
        new Vector3(-0.647f, 0.1f, 0.172f), new Vector3(-0.637f, 0.1f, 0.677f)
    };

    public FarmSceneConfig(){
        validConfigValues = new Dictionary<string, object>();
        int[] nroTomates = new int[3] { 2, 3, 4 };
        int[] nroPeras = new int[3] { 2, 3, 4 };
        validConfigValues.Add("nroTomates", nroTomates);
        validConfigValues.Add("nroPeras", nroPeras);
    }

    public override void loadConfig(Dictionary<string, object> config) {
        GameObject spawnPoint = GameObject.FindWithTag("SpawnPoint");
        long nroTomates = 4;
        long nroPeras = 4;
        if (config.ContainsKey("nroTomates")) {
            nroTomates = (long) config["nroTomates"];
        }
        if (config.ContainsKey("nroPeras")) {
            nroPeras = (long) config["nroPeras"];
        }

        List<int> availableSpawns = new List<int> {0, 1, 2, 3, 4, 5, 6, 7};
        System.Random rand = new System.Random();
        if (tomatePrefab != null) {
            for (int i = 0; i < nroTomates; i++) {
                int spawnIndex = rand.Next(0, availableSpawns.Count);
                GameObject apple = Instantiate(tomatePrefab, spawnPoint.transform);
                apple.transform.localPosition = lugaresSpawn[availableSpawns[spawnIndex]];
                availableSpawns.RemoveAt(spawnIndex);
            }
        }

        if (peraPrefab != null) {
            for (int i = 0; i < nroPeras; i++) {
                int spawnIndex = rand.Next(0, availableSpawns.Count);
                GameObject pear = Instantiate(peraPrefab, spawnPoint.transform);
                pear.transform.localPosition = lugaresSpawn[availableSpawns[spawnIndex]];
                availableSpawns.RemoveAt(spawnIndex);
            }
        }
    }
}