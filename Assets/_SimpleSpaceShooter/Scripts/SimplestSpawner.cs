using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimplestSpawner : MonoBehaviour {

    public Transform prefab;
    public Transform spawnPoint;

    public bool keepSpawnedReferences = false;
    List<Transform> spawned = new List<Transform>();

    public bool activateWithUtility = false;
    public ActivateBehaviourUtility utility;

    

    protected void Awake() {
        if (activateWithUtility) {
            if (utility == null) Debug.Log("Assign spawn activator utility.", this);
            else utility.Register(this);
        }
    }

    public void Spawn() {
        if (keepSpawnedReferences)
            spawned.Add(Instantiate(prefab, spawnPoint.position, spawnPoint.rotation) as Transform);
        else Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
    }
}
