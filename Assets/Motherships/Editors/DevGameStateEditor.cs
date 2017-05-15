using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Displays different important properties to allow for fast changes.
/// 
/// Also contains developer console.
/// 
/// 10.5.2017
/// This editor is deprecated, because gun scripts don't use devEnableFire anymore.
/// </summary>
public class DevGameStateEditor : EditorWindow {

    public bool gunFireEnabled = true;
    SimpleGun[] gunsInScene;

    public bool cameraMovementEnabled = true;
    CameraScrollController cameraController;

    public bool hangarsOpen = true;
    SpawnController[] hangarsInScene;

    public bool timerOn = true;
    Timer timeController;

    public bool clockOn = true;
    TimeCounter clockController;

    public bool startupWarningsOn = true;
    OnStartBuffer startupLogController;

    //private UnitsInScene unitsInScene;

    public bool updated = false;


    string devConsoleText;

    bool levelGroupEnabled = true;

    // Add menu named "My Window" to the Window menu
    [MenuItem ("Window/My Window")]
    static void Init () {
        // Get existing open window or if none, make a new one:
        DevGameStateEditor window = (DevGameStateEditor)EditorWindow.GetWindow(typeof(DevGameStateEditor));
        window.Show();
        // find important dev controllers in scene
        
        window.updated = false;
        window.UpdateData();
    }

    void OnGUI () {
        EditorGUILayout.ObjectField(this, typeof(DevGameStateEditor), true);// just a reference to script
        GUILayout.Label("Dev console", EditorStyles.boldLabel);
        devConsoleText = EditorGUILayout.TextField ("Write commands...", devConsoleText);

        levelGroupEnabled = EditorGUILayout.BeginToggleGroup("Level Settings", levelGroupEnabled);
        if (!levelGroupEnabled) { // when level editing group is disabled, revert to default settings
            DefaultSettings();
        }

        gunFireEnabled = EditorGUILayout.Toggle("gunFireEnabled", gunFireEnabled);
        cameraMovementEnabled = EditorGUILayout.Toggle("cameraMovementEnabled", cameraMovementEnabled);
        hangarsOpen = EditorGUILayout.Toggle("hangarsOpen", hangarsOpen);
        timerOn = EditorGUILayout.Toggle("timerOn", timerOn);
        clockOn = EditorGUILayout.Toggle("clockOn", clockOn);
        // old, move newest one up
        startupWarningsOn = EditorGUILayout.Toggle("startupWarningsOn", startupWarningsOn);

       // EditorGUILayout.ObjectField(unitsInScene, typeof(UnitsInScene), true);// just a reference to script

        updated = false;
        UpdateData();
        EditorGUILayout.EndToggleGroup ();
    }

    void DefaultSettings() {
        gunFireEnabled =
        cameraMovementEnabled =
        hangarsOpen =
        timerOn = true;

        clockOn = false;
    }

    void UpdateData() {
        if (!updated) {
            cameraController = GameObject.FindObjectOfType<CameraScrollController>();
           // unitsInScene = UnitsInScene.scene;
            gunsInScene = GameObject.FindObjectsOfType<SimpleGun>();
            hangarsInScene = GameObject.FindObjectsOfType<SpawnController>();
            timeController = GameObject.FindObjectOfType<Timer>();
            clockController = GameObject.FindObjectOfType<TimeCounter>();
            // old, move newest one up
            startupLogController = GameObject.FindObjectOfType<OnStartBuffer>();

            if (cameraController) {
                cameraController.enabled = cameraMovementEnabled;
            }

            /*if (gunsInScene != null && gunsInScene.Length > 0) {
                foreach (SimpleGun gun in gunsInScene) {
                    gun.devDisableFire = !gunFireEnabled;
                }
            }*/
            if (hangarsInScene != null && hangarsInScene.Length > 0) {
                foreach (SpawnController hangar in hangarsInScene) {
                    hangar.enabled = hangarsOpen;
                }
            }

            if (timeController) 
                timeController.timerOn = timerOn;

            if (clockController)
                clockController.clockOn = clockOn;

            Debug.Log("updating dev state");
            updated = true;
        }
    }
}
