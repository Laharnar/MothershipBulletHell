using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class PublicClickableObjectTagEditor : EditorWindow
{

    ClickableObject[] objs;
    Vector2 scrollView;

    [MenuItem("Utility/Clickable obj tags")]
    static void Init()
    {
        PublicClickableObjectTagEditor win = GetWindow<PublicClickableObjectTagEditor>();
        win.Reload();

    }

    // Update is called once per frame
    void OnGUI()
    {
        if (GUILayout.Toggle(false, "reload"))
        {
            Reload();
        }
        scrollView=EditorGUILayout.BeginScrollView(scrollView);
        for (int i = 0; i < objs.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();
            ShowWithParents(objs[i].transform);
            objs[i].clickTag = EditorGUILayout.TextField(objs[i].clickTag);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }

    private void Reload()
    {
        objs = GameObject.FindObjectsOfType<ClickableObject>();
    }

    void ShowWithParents(Transform t)
    {
        if (t.parent)
        {
            ShowWithParents(t.parent);
        }
        EditorGUILayout.ObjectField(t.transform, typeof(Transform));
    }
}
