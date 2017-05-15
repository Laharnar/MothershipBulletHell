using UnityEngine;
using System.Collections;

public class MoveInDirection : SimplestMovement {

    public float dirDegrees;

	// Use this for initialization
	void Start () {
        //transform.LookAt(transform.position + direction, transform.forward);
        transform.Rotate(new Vector3(0, 0, dirDegrees));
    }
}
