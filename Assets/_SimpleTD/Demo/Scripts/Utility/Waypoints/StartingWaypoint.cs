using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// There can be multiple ones.
/// 
/// Shouldnt have another waypoint on this object
/// </summary>
class StartingWaypoint : Waypoint {

    void Start() {
        StartCoroutine(DrawPath());
        

    }



    public IEnumerator DrawPath() {
        while (true) {
            yield return DrawAllConnections(0);
            yield return new WaitForSeconds(1);
        }
    }
}