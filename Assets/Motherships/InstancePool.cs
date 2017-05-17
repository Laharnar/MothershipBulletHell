using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Pools destroyed and instatiated units into the pool.
/// </summary>
public class InstancePool {

    public enum PoolingMode {
        Destroy,
        Move
    }
    
    // <name of pref, instantiated instances>
    /// <summary>
    /// Don't edit values inside by hand.
    /// </summary>
    public static Dictionary<string, StackOfInstances> pool = new Dictionary<string, StackOfInstances>();

    public class StackOfInstances: Stack<Transform> {
        public int max = 0;

        public new void Push(Transform item) {
            base.Push(item);
            if (max < Count) {
                max = Count;
            }
        }
    }

    public static Transform CreateUnit(Transform pref, PoolingMode mode) {
        Debug.Log(pool+ " "+pref);
        if (!pool.ContainsKey(pref.name)) {
            pool.Add(pref.name, new StackOfInstances());
        }

        if (pool[pref.name].Count > 0) {
            Transform t = pool[pref.name].Peek();
            if (mode == PoolingMode.Destroy) {
                t.gameObject.SetActive(true);
            } else if (mode == PoolingMode.Move){
            } else {
                Debug.Log("Unhandled pooling type");
                return null;
            }
            t.name = t.name.Substring(8);
            return pool[pref.name].Pop();
        } else
            return UnityEngine.Object.Instantiate(pref) as Transform;
    }

    public static int GetMaxCountForType(string key) {
        return pool[key].max;
    }
    public static void PoolExistingInstance(Transform go) {
        string name = go.name;
        go.name = "[Pooled]" + go.name;
        if (name.LastIndexOf("(Clone)") != -1)
            name = name.Substring(0, name.Length - ("(Clone)".Length));
        PoolExistingInstance(name, go);
    }

    public static void PoolExistingInstance(string key, Transform go) {
        if (!pool.ContainsKey(key)) {
            pool.Add(key, new StackOfInstances());
        }
        pool[key].Push(go);
    }

    public static Vector3 NextPoolPosition() {
        return new Vector3(pool.Count * 15, -10000, 0);
    }
}



/// <summary>
/// Base class for objects that should be use pooled instatiate or pooled destroy.
/// </summary>
public abstract class PooledMonoBehaviour : MonoBehaviour {
   
    /// <summary>
    /// Pools object for later use.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="life"></param>
    protected static new void Destroy(UnityEngine.Object obj, float life) {
        UnityEngine.Object.Destroy(obj, life);
        Debug.Log("Redo timed destroys, since they can't be pooled.");
    }

    protected static new void Destroy(UnityEngine.Object obj) {
        Destroy(obj, InstancePool.PoolingMode.Destroy);
    }

    /// <summary>
    /// Pools object for later use.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="life"></param>
    protected static void Destroy(UnityEngine.Object obj, InstancePool.PoolingMode mode) {

        Transform tr = null;
        if (obj.GetType() == typeof(GameObject)) {
            GameObject go = (GameObject)obj;
            tr = go.transform;
        }
        else if (obj.GetType() == typeof(Transform)) {
            tr = (Transform)obj;
        }
        if (tr != null) {
            switch (mode) {
                case InstancePool.PoolingMode.Destroy:
                    tr.gameObject.SetActive(false);
                    break;
                case InstancePool.PoolingMode.Move:
                    tr.position = InstancePool.NextPoolPosition();
                    break;
                default:
                    Debug.Log("unhandled err");
                    break;
            }
            InstancePool.PoolExistingInstance(tr);
        } else {
            UnityEngine.Object.Destroy(obj);
        }
    }

    protected new static UnityEngine.Object Instantiate(UnityEngine.Object obj) {
        return Instantiate(obj, InstancePool.PoolingMode.Destroy);
    }

    /// <summary>
    /// Creates object and pools it.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    protected static UnityEngine.Object Instantiate(UnityEngine.Object obj, InstancePool.PoolingMode mode) {
        if (obj.GetType() == typeof(GameObject)) {
            GameObject go = (GameObject)obj;
            return InstancePool.CreateUnit(go.transform, mode);
        } else if (obj.GetType() == typeof(Transform)) {
            Transform tr = (Transform)obj;
            return InstancePool.CreateUnit(tr, mode);
        } else {
            return UnityEngine.Object.Instantiate(obj);
        }
    }
}