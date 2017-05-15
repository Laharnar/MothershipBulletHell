using UnityEngine;
using System.Collections;

public class StartupCommands : MonoBehaviour {

    public string[] commands;

	// Use this for initialization
	void Start () {
        TurretBuildControl tbc = GetComponent<TurretBuildControl>();
        for (int i = 0; i < commands.Length; i++)
        {
            tbc.OnMousePressedDown(commands[i]);
        }


    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
