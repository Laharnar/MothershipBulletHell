using UnityEngine;

/// <summary>
/// Put it on any child of mothership with collision and link it to correct hp.
/// Bullets need it to apply damage.
///
/// This script can also be extended to ignore damage, or have different behaviour, while collisions stay the same.
/// Works together with DirectCollision.
/// </summary>
public class CollisionTrigger : MonoBehaviour {
    // who gets damaged
    public HpControl source;

    public void ReceiveCollision(DirectCollision sender) {
        Debug.Log(sender.damage);
        source.Damage(sender.damage);
    }
}