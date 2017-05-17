using UnityEngine;
using System.Collections;

// hangars are pooled
public class SpawnController : MonoBehaviour {

    [System.Obsolete("Don't use this anymore, every hangars has this.")]
    public Transform defaultType;

    public Hangar[] hangars;

    public bool useDefaultBehaviour = true;

    private void Awake() {
        if (defaultType == null) {
            Debug.Log("defaultType is null. note, its deprecated, used only on carriers", this);
        }
        if (hangars.Length == 0) {
            Debug.LogWarning("No hangars on spawner.", this);
        }
        if (useDefaultBehaviour) {
            Debug.Log("Default deprecated behaviour is active.");
        }
    }

    void Start()
    {
        if (useDefaultBehaviour)
            StartCoroutine(BuildAndSpawn());
    }

    private IEnumerator BuildAndSpawn() {
        while (true) {
            if (enabled) {
                for (int i = 0; i < hangars.Length; i++) {
                    for (int j = 0; j < 2; j++) {
                        AddOrder(defaultType, hangars);
                    }
                }
            }
            yield return new WaitForSeconds(2);// change this to builder queue type
        }
    }

    /// <summary>
    /// Add new ship to build to the least busy hangar.
    /// </summary>
    /// <param name="prefab"></param>
	public static void AddOrder (Transform prefab, Hangar[] hangars) {
        Hangar h = GetLeastBusyHangar(hangars);
        h.Spawn(prefab);
	}

    /// <summary>
    /// Adds hangars default prefab to spawn list
    /// </summary>
    /// <returns></returns>
    public static void AddOrder(Hangar[] hangars) {
        GetLeastBusyHangar(hangars).SpawnDefault();
    }

    public static Hangar GetLeastBusyHangar(Hangar[] hangars)
    {
        
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
