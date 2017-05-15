using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Unit))]
public class UnitEditor : Editor {

    Unit unit;

    public void Awake() {
        unit = (Unit)target;
        unit.Init();
    }

    public override void OnInspectorGUI() {

        EditorGUILayout.LabelField("Is trigger["+unit.collider.isTrigger+"]");
        EditorGUILayout.LabelField("Has rigidbody[" + unit.rig + "]");
        EditorGUILayout.LabelField("Is kinematic[" + unit.rig.isKinematic + "]");
        EditorGUILayout.PrefixLabel("Lives Worth");
        unit.livesWorth = EditorGUILayout.IntField(unit.livesWorth);

        if (GUI.changed) {
            EditorUtility.SetDirty(unit);
        }

    }
}
