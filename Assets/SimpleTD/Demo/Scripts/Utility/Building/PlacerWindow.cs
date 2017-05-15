using UnityEngine;
using System.Collections;
using UnityEditor;

public class PlacerWindow : EditorWindow {

    enum BuildMode {
        None,
        Waypoints,
        BuildPositions

    }

    BuildMode editMode;

    BuildLocationPlacer tryFindLocationBuilder;
    WaypointPlacer tryFindWaypointPlacer;

	[MenuItem("Level builder/wp-build position")]
	static void Init () {
        PlacerWindow win = EditorWindow.GetWindow<PlacerWindow>();
        win.Show();

        win.Search();
	}

    private void Search() {
        bool spawned = false;
        GameObject go = new GameObject(); ;
        if (!tryFindLocationBuilder) {
            tryFindLocationBuilder = GameObject.FindObjectOfType<BuildLocationPlacer>();

            if (!tryFindLocationBuilder) {
                spawned = true;
                go.AddComponent<BuildLocationPlacer>();
                tryFindLocationBuilder = go.GetComponent<BuildLocationPlacer>();
            }
        }
        if (!tryFindWaypointPlacer) {
            tryFindWaypointPlacer = GameObject.FindObjectOfType<WaypointPlacer>();

            if (!tryFindWaypointPlacer) {
                spawned = true;
                go.AddComponent<WaypointPlacer>();
                tryFindWaypointPlacer = go.GetComponent<WaypointPlacer>();
            }
        }
        if (!spawned) {
            DestroyImmediate(go);
        }
    }
	// Update is called once per frame
	void OnGUI() {
        Search();
        
        bool wps = EditorGUILayout.ToggleLeft("wp build mode", editMode == BuildMode.Waypoints);
        bool towers = EditorGUILayout.ToggleLeft("turret setup mode", editMode == BuildMode.BuildPositions);
        if (towers) {
            editMode = BuildMode.BuildPositions;
        } else if (wps) {
            editMode = BuildMode.Waypoints;
        } else
            editMode = BuildMode.None;

        
        switch (editMode) {
            case BuildMode.None:
                tryFindLocationBuilder.enabled = false;
                tryFindWaypointPlacer.enabled = false;
                break;
            case BuildMode.Waypoints:
                tryFindWaypointPlacer.enabled = true;
                break;
            case BuildMode.BuildPositions:
                tryFindLocationBuilder.enabled = true;
                break;
            default:
                break;
        }
	}
}
