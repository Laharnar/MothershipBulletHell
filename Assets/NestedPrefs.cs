using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;

// 11.5.2017
// should allow you to reparent a bunch of objects from edit mode
public class NestedPrefs : MonoBehaviour {
    
    public string[] children;
    static bool init = false;

    private void Awake() {
        if (!init) {
            Nested.Reload();
            Nested.Connect();
            init = true;
        }
    }
}


public class NestedChild {
    internal Transform parent;
    internal string child;

    public NestedChild(Transform parent, string child) {
        this.parent = parent;
        this.child = child;
    }
}
public class Nested : UnityEditor.EditorWindow{

    static Nested target;

    public static List<NestedChild> connections = new List<NestedChild>();
    public static Dictionary<string, NestedPrefs> parents = new Dictionary<string, NestedPrefs>();

    [MenuItem("MyUtility/Nested prefs")]
    static void Init() {
        target = GetWindow<Nested>();
    }


    public static void Reload() {
        Nested.connections = new List<NestedChild>();
        Nested.parents = new Dictionary<string, NestedPrefs>();
        NestedPrefs[] prefs = GameObject.FindObjectsOfType<NestedPrefs>();
        for (int i = 0; i < prefs.Length; i++) {
            if (prefs[i].children.Length > 0) {
                if (Nested.parents == null) {
                    Nested.parents = new Dictionary<string, NestedPrefs>();
                }

                string fixedName = GetName(prefs[i].name);
                Nested.parents[fixedName] = prefs[i];
                Stack<string> t = new Stack<string>();
                foreach (var item in prefs[i].children) {
                    Nested.connections.Add(new NestedChild(prefs[i].transform, item));
                }
            }
        }
    }

    private static string GetName(string name) {

        // remove (n) at the end of name
        string fixedName = name;
        if (fixedName.EndsWith(")")) {
            int x = fixedName.LastIndexOf('(') - 1;// remove whitespace
            fixedName = fixedName.Substring(0, x);
        }
        return fixedName;
    }

    private void OnGUI() {
        GUILayout.Label("Join prefs.");
        List<NestedChild> dict = connections;
        foreach (var item in dict) {
            GUILayout.Label("- "+item.parent+"/"+item.child);
        }
        if (GUILayout.Button("Reload")) {
            Reload();
        }
        if (GUILayout.Button("Connect")) {
            Connect();
        }

        EditorGUILayout.ObjectField(Selection.activeGameObject, typeof(GameObject), true);
        pref = (Transform)EditorGUILayout.ObjectField(pref, typeof(Transform), false);

        replaceOnlyInThisLayer = EditorGUILayout.Toggle("Replace only in parent", replaceOnlyInThisLayer);
        if (GUILayout.Button("Replace all")) {
            Transform[] t=null;
            if (replaceOnlyInThisLayer) {
                if (Selection.activeGameObject.transform.parent != null) {
                    t = Selection.activeGameObject.transform.parent.GetComponentsInChildren<Transform>();
                }
            }
            else t= GameObject.FindObjectsOfType<Transform>();
            Replace(Selection.activeObject.name, pref, t);
        }

        this.Repaint();

    }
    public Transform pref;
    public bool replaceOnlyInThisLayer=false;

    public static void Connect() {
        // take all parents and resolve their connections
        Transform[] ts = GameObject.FindObjectsOfType<Transform>();
        Dictionary<string, Stack<Transform>> sorted;
        sorted = SortByPath(ts);
        foreach (var item in sorted) {
            //Debug.Log("s:" + item.Value);

        }
        for (int i = 0; i < connections.Count; i++) {
            NestedChild c = connections[i];
            string childKey = c.child;
            Transform parentKey = c.parent;
            //Debug.Log("p:"+parentKey, parentKey);
            if (sorted[childKey].Count > 0) {
                Transform t = sorted[childKey].Pop();
                t.parent = parentKey;
                t.position = parentKey.position;
            } else {
                Debug.Log("not enough items");
            }
        }
    }

    public static void Replace(string name, Transform pref, Transform[] set) {
        if (set == null) return;
        Transform[] t= set;
        for (int i = 0; i < t.Length; i++) {
            // note: last version didn't check for nulls
            if (t[i] != null && GetName(t[i].name) == name) {
                Transform p = Instantiate(pref) as Transform;
                p.position = t[i].position;
                p.rotation = t[i].rotation;
                p.localScale = t[i].localScale;
                p.parent = t[i].parent;
                DestroyImmediate(t[i].gameObject);

            }
        }
    }

    private static string Path(Transform transform) {
        return GetName(transform.name);
    }

    static Dictionary<string, Stack<Transform>> SortByPath(Transform[] ts) {
        Dictionary<string, Stack<Transform>> found = new Dictionary<string, Stack<Transform>>();
        // sort all transforms by their paths

        for (int k = 0; k < ts.Length; k++) {
            string path = Path(ts[k].transform);
            if (!found.ContainsKey(path))
                found.Add(path, new Stack<Transform>());
            found[path].Push(ts[k].transform);
        }
        return found;
    }
}