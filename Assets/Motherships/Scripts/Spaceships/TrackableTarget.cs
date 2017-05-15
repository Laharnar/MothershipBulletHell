using UnityEngine;
using System.Collections;

public class TrackableTarget : MonoBehaviour {

    public bool allowNull = false;

    /// <summary>
    /// Simple reference to source
    /// </summary>
    public UnitInfo shipSource;

    /// <summary>
    /// This defines who many guns at maximum should follow this target.
    /// </summary>
    public int targetingPriority;

    void Awake() {
        if (!allowNull && shipSource == null) {
            Logger.AddLog("Assign shipSource or allow null", this);
        }
    }
}
