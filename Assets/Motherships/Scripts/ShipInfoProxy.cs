using UnityEngine;
using System.Collections;

/// <summary>
/// Just a connection to ship info, use for collisions on child objects
/// </summary>
public class ShipInfoProxy : MonoBehaviour {

    public ShipInfo source;

    void Awake() {
        if (source == null) {
            source = GetComponentInParent<ShipInfo>();
            Debug.LogWarning("Manualy assign source.");
        }
    }
}
