using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class QuickFindObjectWindow : EditorWindow {

    // lets you quickly find and select objects
    [SerializeField]
    List<StrObj> quickFindList;

    static QuickFindObjectWindow win;

    [MenuItem("Utility/Simple selector")]
	static void Init () {
        win = EditorWindow.GetWindow<QuickFindObjectWindow>();
        win.quickFindList = new List<StrObj>();
        Selection.selectionChanged += win.OnSelectionChange;
        win.Show();
	}

    void OnSelectionChange() {
        OnGUI();
    }

	// Update is called once per frame
	void OnGUI () {
        if (!win) {
            return;
        }
        win.Repaint();
        GameObject selectionChange = Selection.activeGameObject;
        string tagSelected = "";
        EditorGUILayout.BeginVertical();

        if (selectionChange != null) {
            EditorGUILayout.ObjectField(selectionChange, selectionChange.GetType(), true);
            tagSelected = EditorGUILayout.TextField(tagSelected);
            if (GUILayout.Button("Save")) {
                quickFindList.Add(new StrObj(selectionChange, tagSelected));
            }
        } else {
            GUILayout.Space(30);
            GUILayout.Label("Nothing is selected");

        }

        if (quickFindList.Count == 0) {
            GUILayout.Label("No entries");
        }
        // draw quick access list
        for (int i = 0; i < quickFindList.Count; i++) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(quickFindList[i].selectionChange, quickFindList[i].selectionChange.GetType(), true);

            tagSelected = EditorGUILayout.TextField(quickFindList[i].tagSelected);
            bool change = quickFindList[i].changed;
            quickFindList[i].changed = EditorGUILayout.Toggle(quickFindList[i].changed);
            if (change != quickFindList[i].changed) {
                Selection.activeObject = quickFindList[i].selectionChange;
            }
            if (GUILayout.Button("Remove")) {
                quickFindList.RemoveAt(i); 
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
        win.Repaint();
        if (GUI.changed) {
            EditorUtility.SetDirty(win);
        }
	}
}
[System.Serializable]
internal class StrObj {

    internal GameObject selectionChange;
    internal string tagSelected;
    internal bool changed = false;

    internal StrObj(GameObject selectionChange, string tagSelected) {
        this.selectionChange = selectionChange;
        this.tagSelected = tagSelected;
    }
}