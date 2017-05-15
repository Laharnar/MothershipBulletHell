using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Requires Path Node scripts on as child components
/// 
/// Use trigger collider together with this script to get nodes for the path
/// </summary>
public class CircularPath : MonoBehaviour {

    PathNode[] nodes;

	// Use this for initialization
	void Awake () {
        nodes = GetComponentsInChildren<PathNode>();
	}

    PathNode[] GetNodes() {
        return nodes;
    }

    /// <summary>
    /// Gets distances between all nodes and vector
    /// </summary>
    /// <returns></returns>
    public float[] GetDistances(Vector3 from) {
        float[] dists = new float[nodes.Length];
        for (int i = 0; i < dists.Length; i++) {
            dists[i] = Vector3.Distance(from, nodes[i].transform.position);
        }
        return dists;
    }

    /// <summary>
    /// Constructs a simple path with limited length, by taking first subnode from every node.
    /// </summary>
    /// <param name="start">if null, return empty path</param>
    /// <param name="maxLength">start node is included in count</param>
    public QuePath GetPath(PathNode start, int maxLength) {
        QuePath p = new QuePath();
        List<PathNode> sub = new List<PathNode>();
        GetSubNodes(start, 0, maxLength, ref sub);
        foreach (var s in sub) {
            p.AddNode(s);
        }
        return p;
    }

    public void GetSubNodes(PathNode node, int curDepth, int maxDepth, ref List<PathNode> array) {
        
        if (curDepth >= maxDepth) {
            return;    
        }
        if (node != null) {
            array.Add(node);
        }
        if (node.subnodes.Length == 0) {
            Debug.Log("Node doesn't have connections assigned.", node);
        } else {
            GetSubNodes(node.subnodes[0], curDepth + 1, maxDepth, ref array);
        }
    }

    /// <summary>
    /// returns closes node to the vector
    /// </summary>
    /// <param name="vector3"></param>
    /// <returns></returns>
    internal PathNode Closest(Vector3 vector3) {
        float minDist = 100000000000000;
        int t = -1;
        for (int i = 0; i < nodes.Length; i++) {
            float sqdist = (vector3-nodes[i].transform.position).sqrMagnitude;
            if (sqdist<minDist) {
                minDist = sqdist;
                t = i;
            }
        }
        return nodes[t];
    }
}


public class QuePath {
    //* path class uses directly nodes instead of their subnodes *//


    Queue<PathNode> nodes;

    string pathName;

    internal int startingNumOfNodes = 0;

    public int UsingPath { get; internal set; }

    public QuePath(string pathName = "none") {
        this.nodes = new Queue<PathNode>();
        this.pathName = pathName;
    }

    public void AddNode(PathNode node) {
        nodes.Enqueue(node);
    }

    public void StartOnPath() {
        Debug.Log("Starting path");
        startingNumOfNodes = nodes.Count;
        UsingPath = 1;
    }

    /// <summary>
    /// Dequeues next node
    /// </summary>
    /// <returns></returns>
    public PathNode NextNode() {
        if (nodes.Count == 0) {
            Debug.Log("Zero nodes in path.");
            return null;
        }
        return nodes.Dequeue();
    }

    internal int NodeCount() {
        return nodes.Count;
    }
}
