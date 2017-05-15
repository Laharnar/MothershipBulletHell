using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// Displays information about pooled objects.
/// </summary>
public class PoolingGraph : EditorWindow {

    [MenuItem("MyUtility/Pooling")]
    static void Init() {
        EditorWindow window = GetWindow(typeof(PoolingGraph));
        window.Show();
    }

    private void OnGUI() {
        GUILayout.Label("Displays all pooled objects");
        if (!Application.isPlaying) {
            EditorGUILayout.LabelField("Press play...");
        }
        GUILayout.Label("Counted pooled instances for all types.");

        foreach (var i in InstancePool.pool) {
            GUILayout.Label(i.Key + " c:" + i.Value.Count + " max:" + InstancePool.GetMaxCountForType(i.Key));
        }

        this.Repaint();
    }
}
