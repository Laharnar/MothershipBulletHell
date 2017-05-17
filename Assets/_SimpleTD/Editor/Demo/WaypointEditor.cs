using UnityEditor;

//[UnityEditor.CustomEditor(typeof(Waypoint))]
public class WaypointEditor : Editor {

    public override void OnInspectorGUI() {
        Waypoint waypoint = (Waypoint)target;

        for (int i = 0; i < waypoint.subpoints.Count; i++) {
            //EditorGUILayout.ObjectField();
        }
    }
}


