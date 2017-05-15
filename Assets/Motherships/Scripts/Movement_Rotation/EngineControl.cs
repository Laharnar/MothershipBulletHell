using UnityEngine;
using System.Collections;

public class EngineControl : MonoBehaviour {
    public Engine[] engines;
    public SideEngine[] sideEngines;


    internal void SuspendAll() {
        for (int i = 0; i < engines.Length; i++) {
            if (engines[i]) {
                engines[i].Suspend();
            } else {
                Debug.LogWarning("[EngineControl]empty engine slot", this);
            }
        }

        for (int i = 0; i < sideEngines.Length; i++) {
            if (sideEngines[i]) {
                sideEngines[i].Suspend();
            } else {
                Debug.LogWarning("[EngineControl]empty engine slot", this);
            }
        }
    }

    internal void ResumeAll() {
        for (int i = 0; i < engines.Length; i++) {
            if (engines[i]) {
                engines[i].Resume();
            } else {
                Debug.LogWarning("[EngineControl]empty engine slot", this);
            }
        }

        for (int i = 0; i < sideEngines.Length; i++) {
            if (sideEngines[i]) {
                sideEngines[i].Resume();
            } else {
                Debug.LogWarning("[EngineControl]empty engine slot", this);
            }
        }
    }

    internal void RegisterFollower(Transform source, System.Action<EngineControlPanel, Vector3, Vector3> OnPosChange, System.Action<EngineControlPanel, Vector3, Vector3> OnRotChange) {
        for (int i = 0; i < engines.Length; i++) {
            if (engines[i]) {
                engines[i].RegisterOnMove(source, OnPosChange);
            } else {
                Debug.LogWarning("[EngineControl]empty engine slot", this);
            }
        }
        for (int i = 0; i < sideEngines.Length; i++) {
            if (sideEngines[i]) { 
                sideEngines[i].RegisterOnRotation(source, OnRotChange); 
            } else {
                Debug.LogWarning("[EngineControl]empty engine slot", this);
            }
        }
    }

    internal void CancelRegistration(Transform source) {
        
        for (int i = 0; i < engines.Length; i++) {
            if (engines[i]) {
                engines[i].CancelRegistration(source);
            } else {
                Debug.LogWarning("[EngineControl]empty engine slot", this);
            }
        }
        for (int i = 0; i < sideEngines.Length; i++) {
            if (sideEngines[i]) {
                sideEngines[i].CancelRegistration(source);
            } else {
                Debug.LogWarning("[EngineControl]empty engine slot", this);
            }
        }
    }
}
