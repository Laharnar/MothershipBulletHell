using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Waypoint : MonoBehaviour {

    public List<Waypoint> subpoints;

    static List<Waypoint> fullPathWithoutLoops;

    public MonoBehaviour additionalTarget;// in here put scripts that make use of this waypoint system

    protected IEnumerator DrawAllConnections(int depth) {
        if (depth == 0)
	    {
            fullPathWithoutLoops = new List<Waypoint>();
	    }
        if (depth < 100) {
            fullPathWithoutLoops.Add(this);
            CheckNulls();
            for (int i = 0; i < subpoints.Count; i++) {
                if (fullPathWithoutLoops.Contains(subpoints[i])) {
                    // there is a loop, so ignore it, just warn about it. It can be false positive if multiple paths lead into 1.
                    Debug.DrawLine(transform.position, subpoints[i].transform.position, Color.red, 2);
                } else {
                    Debug.DrawLine(transform.position, subpoints[i].transform.position, Color.green, 1);
                }
                yield return new WaitForSeconds(0.05f);
            }
            for (int i = 0; i < subpoints.Count; i++) {
                yield return subpoints[i].DrawAllConnections(depth + 1);
            }
        }
        
    }

    /// <summary>
    /// Chose one assuming it exists
    /// </summary>
    /// <returns></returns>
    public Waypoint ChoseOne() {
        if (subpoints == null || subpoints.Count == 0) {
            return null;
        }
        
        CheckNulls();
        int index = Random.Range(0, subpoints.Count - 1);
        return subpoints[index];
    }


    internal void AddWp(Waypoint wp) {
        CheckNulls();

        if (subpoints.Contains(wp)) {
            return;
        }
        subpoints.Add(wp);
    }

    internal void CheckNulls() {
        for (int i = 0; i < subpoints.Count; i++) {
            if (subpoints[i] == null) {
                subpoints.RemoveAt(i);
                i--;
            }
        }
    }

    internal void RemoveConnection(Waypoint wp) {
        if (subpoints.Contains(wp)) {
            subpoints.Remove(wp);
        }
    }
}

