using UnityEngine;
using System.Collections;

/// <summary>
/// Registers object as a target that can be searched.
/// </summary>
[System.Obsolete("Use TargetableBase instead")]
public class TrackableTarget : MonoBehaviour {
    public bool allowNull = false;

    /// <summary>
    /// Simple reference to source
    /// </summary>
    public UnitInfo shipSource;

    /// <summary>
    /// This defines who many guns at maximum should follow this target.
    /// </summary>
    public int targetingPriority=1;

    void Awake() {
        if (!allowNull && shipSource == null) {
            Logger.AddLog("Assign shipSource or allow null", this);
        }
    }
}
