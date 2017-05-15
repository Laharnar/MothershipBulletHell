using UnityEngine;
using System.Collections;

public class SpawnerPolicy : MonoBehaviour {
    public static EvenSpawn evenSpawn;

    protected virtual void UpdateSpawn(Transform[] spawnPoints) {
        throw new System.NotImplementedException();
    }
}
