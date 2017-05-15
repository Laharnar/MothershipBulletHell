using UnityEditor;

//[CustomPropertyDrawer(typeof(Waypoint))]
public class WaypointProperty : PropertyDrawer {
    public override void OnGUI(UnityEngine.Rect position, SerializedProperty property, UnityEngine.GUIContent label) {
        label.text = "text";
        base.OnGUI(position, property, label);
        // TODO: add remove button to every item, which removes item from array. maybe editor is better for this, google it
        // edit array  with property drawer
        // TODO; at the end add button add new item, which adds empty item into array
    }
}