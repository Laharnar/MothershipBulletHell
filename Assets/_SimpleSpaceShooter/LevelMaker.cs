using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[ExecuteInEditMode]
public class LevelMaker : MonoBehaviour {

    List<Transform> spawned = new List<Transform>();

    public Transform[] brushes;
    public int activeBrush;

    public List<Transform> Spawned { get { return spawned; } set { spawned = value; } }
    public int ActiveBrush { get { return activeBrush; } set { activeBrush = value; } }

    public int IsPositionUsed(Vector3 snapped) {
        for (int i = 0; i < spawned.Count; i++) {
            if (spawned[i] == null) spawned.RemoveAt(i--);
            if (spawned[i].position == snapped) {
                return i;
            }
        }
        return -1;
    }
}
