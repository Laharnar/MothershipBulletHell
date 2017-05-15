using UnityEngine;
using System.Collections;
using UnityEditor;


public class CamEditor : EditorWindow {

    bool camToggle = true;

    [MenuItem("MyEditors/CamEditor")]
	static void Init () {
        CamEditor win = GetWindow<CamEditor>();
	}
	
	// Update is called once per frame
	void OnGUI () {
        camToggle = EditorGUILayout.BeginToggleGroup("Camera Settings", camToggle);
        EditorGUILayout.ObjectField(Camera.main, typeof(Camera), true);
        Camera.main.orthographicSize = EditorGUILayout.FloatField("Orthographic size", Camera.main.orthographicSize);
        EditorGUILayout.EndToggleGroup();
	}
}
