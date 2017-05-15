using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Makes sure object is registered as unit in the scene.
/// </summary>
[System.Obsolete("is this script used?")]
public class RegisterInScene : MonoBehaviour {

    [SerializeField]
    bool applied = false;

	// Use this for initialization
	void Awake ()
    {
        if (!applied && !TryRegister())
        {
            StartCoroutine(KeepChecking());
        }
    }

    private IEnumerator KeepChecking()
    {
        while (!applied)
        {
            yield return new WaitForSeconds(10);
            TryRegister();
        }
    }

    private bool TryRegister()
    {
        //UnitsInScene scene = GameObject.FindObjectOfType<UnitsInScene>();
        //if (scene)
        //{
            //scene.RegisterUnit(transform);
            UnitsInScene.RegisterUnit(transform);
            applied = true;
            return true;
        //}
        //Debug.Log("Missing 'UnitsInScene' script. Place one somewhere in scene.");
        //applied = false;
        //return false;
    }
}
