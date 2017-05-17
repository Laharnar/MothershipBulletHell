using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Alliance : MonoBehaviour {

    public string alliance = "unknown";
    public bool registerInScene = true;

    void Start()
    {
        if (registerInScene) {
            UnitsInScene.RegisterUnit(transform);
        }
    }
}
