using UnityEngine;
using System.Collections;

/// <summary>
/// Allows developer to disable aiming, firing, etc by disabling values in inspector.
/// </summary>

public class DevGameState: MonoBehaviour {

    public bool gunFireEnabled;
    Gun[] gunsInScene;

    public bool cameraMovementEnabled;
    CameraScrollController cameraController;

    public bool hangarsOpen = true;
    SpawnController[] hangarsInScene;

    public bool timerOn = true;
    Timer timeController;

    public bool updated;

	// Use this for initialization
	void Start () {
        cameraController = GameObject.FindObjectOfType<CameraScrollController>();
        gunsInScene = GameObject.FindObjectsOfType<Gun>();
        hangarsInScene = GameObject.FindObjectsOfType<SpawnController>();
        timeController = GameObject.FindObjectOfType<Timer>();

        Update();
    }
	
	// Update is called once per frame
	void Update () {
        if (!updated) {
            foreach (Gun gun in gunsInScene) {
                gun.enabled = gunFireEnabled;
            }

            cameraController.enabled = cameraMovementEnabled;

            foreach (SpawnController hangar in hangarsInScene) {
                hangar.enabled = hangarsOpen;
            }

            timeController.enabled = timerOn;

            Debug.Log("updating dev state");
            updated = true;
        }
        
	}
}
