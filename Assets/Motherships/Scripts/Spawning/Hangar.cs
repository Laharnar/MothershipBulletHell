using UnityEngine;
using System.Collections;

public class Hangar : PooledMonoBehaviour {

    public Transform defaultPrefab;
    public float rate;
    [System.Obsolete("At the moment it's completly redundant, because we dont track spawned objects.")]
    public int limit = 10;

    /// <summary>
    /// is coroutine for spawning running or not, pause version
    /// note: canceling spawning does not empty queue. it functions like pause
    /// </summary>
    bool spawning = false;

    Queue spawnQueue = new Queue();

    public int QueSize { get { return spawnQueue.Count; } }

    public void SpawnDefault()
    {
        Spawn(defaultPrefab);
    }

    public void Spawn(Transform prefab)
    {
        spawnQueue.Enqueue(prefab);
        if (!spawning)
        {
            StartCoroutine(TrySpawn());
        }
    }

    public bool HasLessOrders(int otherOrderCount)
    {
        return spawnQueue.Count < limit && spawnQueue.Count < otherOrderCount;
    }

    private IEnumerator TrySpawn()
    {
        spawning = true;
        //repeat until all are spawned.
        float nextSpawn = Time.time + rate;
        while (spawnQueue.Count > 0)
        {
            if (Time.time > nextSpawn)
            {
                Transform pref = (Transform)spawnQueue.Dequeue();
                Transform newUnit = Instantiate(pref, InstancePool.PoolingMode.Move) as Transform;
                newUnit.position = transform.position;
                newUnit.rotation = transform.rotation;
                newUnit.GetComponent<HpControl>().Init();

                UnitsInScene.RegisterUnits(newUnit);
                nextSpawn = Time.time + rate;
            }
            yield return null;
        }
        spawning = false;
    }

}
