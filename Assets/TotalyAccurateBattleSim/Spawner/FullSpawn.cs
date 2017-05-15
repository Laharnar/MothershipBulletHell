using UnityEngine;
using System.Collections;

public class FullSpawn : MonoBehaviour {

    public string searchFor = "grid";

    public TransformAlliance[] spawnPrefabs;

    [System.Serializable]
    public class TransformAlliance
    {
        public Transform target;
        public string type;

        public TransformAlliance(Transform target, string type)
        {
            this.target = target;
            this.type = type;
        }
    }

    // Use this for initialization
    void Start () {
        GameObject go = GameObject.Find(searchFor);
        if (go)
        {
            Transform[] children = go.transform.GetChildrenWithoutParent();

            Color[] colors = new Color[children.Length];
            string[] childType = new string[children.Length];
            for (int i = 0; i < children.Length; i++)
            {
                colors[i] = children[i].GetComponent<SpriteRenderer>().color;
                childType[i] = children[i].GetComponent<Alliance>().alliance;
            }
            // spawn based on matched type. ignore unknown
            int lastSpawned = 0;
            string lastSpawnedType = "";
            for (int i = 0; i < children.Length; i++)
            {
                if (childType[i] == "unknown")
                {
                    continue;
                }
                if (childType[i] == lastSpawnedType)
                {
                    Instantiate(spawnPrefabs[lastSpawned].target, children[i].transform.position, children[i].transform.rotation, transform);
                    continue;
                }
                for (int j = 0; j < spawnPrefabs.Length; j++)
                {
                    string matchingType = childType[i];
                    if (matchingType == "unknown")
                    {
                        continue;
                    }
                    if (spawnPrefabs[j].type == matchingType)
                    {
                        lastSpawnedType = spawnPrefabs[j].type;
                        lastSpawned = j;
                        Instantiate(spawnPrefabs[j].target, children[i].transform.position, children[i].transform.rotation, transform);
                        break;
                    }
                }
            }
            Destroy(go);
        }
    }
}