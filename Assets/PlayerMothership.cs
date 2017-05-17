using UnityEngine;
using System.Collections;
using System;

public class PlayerMothership : MothershipBase {
    
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void Destroyed() {
        throw new NotImplementedException("Implement destroy function on player.");
    }

}
