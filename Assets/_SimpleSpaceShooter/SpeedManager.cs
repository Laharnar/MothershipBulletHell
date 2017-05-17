using UnityEngine;
using System.Collections;

public class SpeedManager : MonoBehaviour {

    // keep original data for the future
    public SimplestMovement originalDefaultMovement;
    public SimplestMovement saveInto;

	// Use this for initialization
	void Start () {
        saveInto.movementVertical = originalDefaultMovement.movementVertical;
        saveInto.movementHorizontal= originalDefaultMovement.movementHorizontal;
        saveInto.rotation = originalDefaultMovement.rotation;

	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.I)) {
            originalDefaultMovement.movementVertical.maxSpeed += 0.2f;
        }
        if (Input.GetKeyDown(KeyCode.K)) {
            originalDefaultMovement.movementVertical.maxSpeed = Mathf.Clamp(originalDefaultMovement.movementVertical.maxSpeed -= 0.2f, 0, float.MaxValue);
        }
        originalDefaultMovement.SetMoveLerp(0, 1);

    }
}
