using UnityEngine;
using System.Collections;
using System;

public class SpawnArea : MonoBehaviour {

    Transform t;
    public Transform prefab;
    public int spawnAmount = 1000;
    public Transform parentTarget;
    Vector3 scale;
    Vector3 offset;

    public Vector2 waitTime; // x:min/y:max range
    public float doneTime;
    public int count = 0;
    public int skipFrames = 3;

    UnitInfo[] spawned;

    // Use this for initialization
    void Start () {
        t = transform;
        scale = t.lossyScale;
        offset = t.position - scale/2;

        if (parentTarget == null) {
            parentTarget = t;
        }

        StartCoroutine(SpawnAtRandom());
        
	}

    private IEnumerator SpawnAtRandom() {
        spawned = new UnitInfo[spawnAmount];

        float startTime = Time.time;
        float frameLength = Time.time;
        for (int i = 0; i < spawnAmount; i++) {
            Transform go = Instantiate(prefab);
            go.transform.position = new Vector3(UnityEngine.Random.Range(0, scale.x) + offset.x,
                UnityEngine.Random.Range(0, scale.y) + offset.y,
                UnityEngine.Random.Range(0, scale.z) + offset.z);

            go.parent = parentTarget;
            spawned[i] = go.GetComponent<UnitInfo>();
            count = i;
            if (i % skipFrames == 0)
                yield return new WaitForEndOfFrame();
        }
        doneTime = Time.time - startTime;

        for (int i = 0; i < spawnAmount; i++) {
            if (spawned[i].Get<GroupAttacker>())
                spawned[i].GetLast<GroupAttacker>().enabled = true;
            if (spawned[i].Get<GroupTurret>())
                spawned[i].GetLast<GroupTurret>().enabled = true;

            count = i;
            if (i % skipFrames == 0)
                yield return new WaitForEndOfFrame();
        }
        doneTime = Time.time - startTime;

    }
}
