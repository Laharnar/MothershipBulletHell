using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;

/// <summary>
/// Displays actions and scores on selected ai unit.
/// 
/// Note: Currently it only displays 1 type of ai. Add base class type for ai.
/// </summary>
public class AiGraph : EditorWindow{

    AssaultCraft craft;
    SimpleGun[] gun;

    [MenuItem("Examples/GUILayout TextField")]
    static void Init() {
        EditorWindow window = GetWindow(typeof(AiGraph));
        window.Show();
    }

    void OnGUI() {
        GUILayout.Label("Displays values from utility ai. (Currently supports only 1 unit and 1 gun.)");
        if (!Application.isPlaying) {

            EditorGUILayout.LabelField("Press play...");
        } else {
            EditorGUILayout.TextField("AssaultCraft: 1");
            if (craft == null) {
                craft = GameObject.FindObjectOfType<AssaultCraft>();
            }
            if (gun == null) {
                gun = GameObject.FindObjectsOfType<SimpleGun>();
            }
            EditorGUILayout.TextField("SimpleGun: "+ gun.Length);
            AssaultCraft aiTarget = craft;
            if (aiTarget) {
                EditorGUILayout.TextField("Object Name: ", aiTarget.name);

                // display ai scores
                List<UtilityNode> ut = aiTarget.root.choices;
                for (int i = 0; i < ut.Count; i++) {
                    EditorGUILayout.FloatField(ut[i].method.Method.Name, ut[i].sum);
                }
            }
            if (gun != null && gun.Length > 0) {
                for (int i = 0; i < gun.Length; i++) {
                    if (gun[i] == null) continue;
                    EditorGUILayout.ObjectField(gun[i], typeof(SimpleGun), true);
                    EditorGUILayout.TextField("Object Name: ", gun[i].name);

                    // display ai scores
                    List<UtilityNode> ut = ((DecisionMaxPositiveChoice)gun[i].GetLogic()).choices;
                    for (int j = 0; j < ut.Count; j++) {
                        EditorGUILayout.FloatField(ut[j].method.Method.Name, ut[j].sum);
                    }
                }
            }
            UnitInfo unit = Selection.activeGameObject.GetComponent<UnitInfo>();
            if (unit && unit.Get<AiBase>(typeof(SuicideBomber_v3))) {
                DrawAiScorers(unit.GetLast<AiBase>());
            }
        }
        this.Repaint();
    }

    private void DrawAiScorers(AiBase simpleGun) {
        if (simpleGun == null) return;

        // display ai scores
        DecisionLeaf dl = simpleGun.GetLogic();
        if (dl == null) return;
        List<UtilityNode> ut = dl.choices;
        EditorGUILayout.ObjectField(simpleGun, typeof(SimpleGun), true);
        EditorGUILayout.TextField("Object Name: ", simpleGun.name);
        for (int j = 0; j < ut.Count; j++) {
            EditorGUILayout.FloatField(ut[j].method.Method.Name, ut[j].sum);
        }
    }
}
