using UnityEngine;

/// <summary>
/// Put it on bullets. Less performance, but no problems with sleeping rigs.
/// 
/// Collisions are only on moving bullets. Ships are always receivers.
/// 
/// Works together with CollisionTrigger
/// </summary>
public class DirectCollision : MonoBehaviour {

    /// <summary>
    /// On collision damage
    /// </summary>
    public int damage;

    void OnTriggerEnter2D(Collider2D collision) {
        // cant get via unit info because child collisions might not have it, it's only on parent.
        CollisionTrigger c = collision.GetComponent<CollisionTrigger>();
        if (c) {
            c.ReceiveCollision(this);
        } else Debug.Log("Missing collision trigger on " + collision.transform, collision.transform);
    }
    //void OnTriggerExit2D(Collision2D collision) {

    //}
}
