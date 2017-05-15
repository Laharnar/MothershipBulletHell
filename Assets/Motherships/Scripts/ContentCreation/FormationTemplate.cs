using UnityEditor;
using UnityEngine;

/// <summary>
/// Put it on instatiated empties.
/// 
/// Provides interfave until actual prefab is created.
/// 
/// Just a way to assign ship prefab that was set in formation builder.
/// </summary>
[@RequireComponent(typeof (FormationUnit))]
public class FormationTemplate : MonoBehaviour
{
    // the actual visible unit
    public string name;
    public Transform prefab;
    internal Transform connection;

    public FormationTemplate OnCreateEmpty(FormationTemplate template)
    {
        prefab = template.prefab;
        gameObject.AddComponent<BoxCollider2D>();// for on exit spawn point
        return this;
    }
}