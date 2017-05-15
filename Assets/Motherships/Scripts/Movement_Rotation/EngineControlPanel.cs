
using UnityEngine;
public class MessageCallback {
    public Transform target;
    public System.Action<EngineControlPanel, Vector3, Vector3> callback;

    public MessageCallback(Transform target, System.Action<EngineControlPanel, Vector3, Vector3> callback) {
        this.target = target;
        this.callback = callback;
    }
}

public abstract class EngineControlPanel : UnityEngine.MonoBehaviour {

    /// <summary>
    /// completly stop movement by this object for a while.
    /// Doesn't reset movement by engine to zero
    /// 
    /// Added on 15.9.2016
    /// Moved to separated script on 17.9.2016
    /// </summary>
    protected bool suspended { get; private set; }
    internal void Resume() {
        if (!suspended) {
            return;
        }
        suspended = false;
    }

    internal void Suspend() {
        if (suspended) {
            return;
        }
        suspended = true;
    }
}

