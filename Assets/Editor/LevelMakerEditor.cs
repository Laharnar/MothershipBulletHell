using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(LevelMaker))]
public class LevelMakerEditor : Editor {

    LevelMaker source;

    void OnSceneGUI() {
        // get the chosen game object
        source = target as LevelMaker;

        if (source)
        TryDrawUpdate();
    }

    // Update is called once per frame
    public void TryDrawUpdate() {
        Event e = Event.current;
        var mousePosInScene = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;

        if (Event.current.type == EventType.MouseDown && Event.current.button == 1) {
            Vector3 snapped = Snap(mousePosInScene);
            DeleteIfTaken(snapped);
        } 
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0) {
            //Vector3 world = Camera.main.ScreenToWorldPoint(e.mousePosition);
            //RaycastHit2D raycast2d = Physics2D.Raycast(world, Vector3.forward, Mathf.Infinity, 1 << LayerMask.NameToLayer("DevelopGroundLayer"));
            
            Vector3 snapped = Snap(mousePosInScene);
            Draw(source.ActiveBrush, snapped, DeleteIfTaken(snapped));
        }
    }

    private Vector3 Snap(Vector3 pos) {
        return new Vector3(Mathf.RoundToInt(pos.x),
        Mathf.RoundToInt(pos.y),
        0);
    }

    private int DeleteIfTaken(Vector3 snapped) {// note that key still still exists, just value is null
        int i = source.IsPositionUsed(snapped);
        if (i != -1) {
            if (source.Spawned[i] != null)
            DestroyImmediate(source.Spawned[i].gameObject);
            source.Spawned.RemoveAt(i);
        }
        return i;
    }

    private void Draw(int activeBrush, Vector3 pos, int i) {
        if (i == -1) {
            source.Spawned.Add(null);
            i = source.Spawned.Count - 1;
        }

        Transform t = Instantiate<Transform>(source.brushes[activeBrush]);
        t.position = pos;

        source.Spawned[i] = t;
    }
}
