using UnityEngine;
using System.Collections;
using UnityEditor;

public class DevMakeLeader : EditorWindow {

	[MenuItem("Window/Formation settings")]
	void InitWindow () {
        DevMakeLeader window = (DevMakeLeader)EditorWindow.GetWindow<DevMakeLeader>();

        window.Show();
	}
	
	// Update is called once per frame
	void OnGUI () {
        Transform selectedObject = Selection.activeTransform;
        bool isleader = false;
        if (selectedObject.GetComponent<FormationUnit>()) {
            isleader = true;
            if (GUILayout.Button("Remove leader")) {
                selectedObject.gameObject.GetComponent<FormationUnit>().leader = null;
            }
        }
        if (!isleader && selectedObject.GetComponent<FormationTemplate>()) {
            if (GUILayout.Button("Make leader")) {
                selectedObject.GetComponent<FormationUnit>().leader = selectedObject.GetComponent<FormationUnit>();
            }
        }
	}
}
