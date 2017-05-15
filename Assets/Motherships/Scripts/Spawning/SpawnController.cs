using UnityEngine;
using System.Collections;

// hangars are pooled
public class SpawnController : MonoBehaviour {

    public Transform defaultType;

    public Hangar[] hangars;

    void Start()
    {
        StartCoroutine(BuildAndSpawn());
    }

    private IEnumerator BuildAndSpawn() {
        while (true) {
            if (enabled) {
                for (int i = 0; i < hangars.Length; i++) {
                    for (int j = 0; j < 2; j++) {
                        AddOrder(defaultType);
                    }
                }
            }
            yield return new WaitForSeconds(2);// change this to builder queue type
        }
    }

    /// <summary>
    /// Add new order to the least busy hangar.
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns>true if it was succesful</returns>
	bool AddOrder (Transform prefab) {
        Hangar h = GetLeastBusyHangar();
        h.Spawn(prefab);
        return true;
	}

    private Hangar GetLeastBusyHangar()
    {
        if (hangars.Length == 0)
        {
            Debug.LogWarning("No spawners.", this);
            return null;
        }

        Hangar bestHangar = hangars[0];
        for (int i = 1; i < hangars.Length; i++)
        {
            if (hangars[i].HasLessOrders(bestHangar.QueSize))
            {
                bestHangar = hangars[i];
            }
        }
        return bestHangar;
    }
}
