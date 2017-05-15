using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SideEngine : EngineControlPanel {

    List<MessageCallback> onRotationMessages;

    public Vector3 standardRotationVector = new Vector3(0, 0, 1);

    void Update() {
        OnUpdate();
    }

    protected virtual void OnUpdate() {
        Vector3 lastRotationEuler = transform.eulerAngles;
        Quaternion lastRotation = transform.rotation;

        Steering();

        Quaternion newSaveRotation = transform.rotation;
        Vector3 newRotationEuler = transform.eulerAngles;

        OnRotation(lastRotationEuler, newRotationEuler);
    }

    protected virtual void Steering() {
        transform.Rotate(standardRotationVector);
    }

    public void RegisterOnRotation(Transform source, System.Action<EngineControlPanel, Vector3, Vector3> callback) {
        if (onRotationMessages == null) {
            onRotationMessages = new List<MessageCallback>();
        }
        this.onRotationMessages.Add(new MessageCallback(source, callback));
    }

    public void CancelRegistration(Transform source) {
        if (onRotationMessages == null) {
            return;
        }
        for (int i = 0; i < onRotationMessages.Count; i++) {
            if (onRotationMessages[i].target == source) {
                onRotationMessages.RemoveAt(i);
                i--;
            }
        }
    }

    // move this to whatever layer steering is done
    protected virtual void OnRotation(Vector3 eulerOldRot, Vector3 eulerNewRot) {
        if (onRotationMessages != null) {
            for (int i = 0; i < onRotationMessages.Count; i++) {
                if (onRotationMessages[i] != null) {
                    onRotationMessages[i].callback(this, eulerOldRot, eulerNewRot);
                }
            }
        }
    }
}