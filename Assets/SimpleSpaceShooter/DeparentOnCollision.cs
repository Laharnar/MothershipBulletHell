using UnityEngine;
using System.Collections;

/* Example use:
 * Put this script on level end with.
 * Setup this and another collision to collide.
 * Set parent with some other behaviour
 * 
 * Set other's collision tag to same as this utility.
 * */
 /// <summary>
 /// Disables movement on collided and deparent it to other object (probably with different behaviour).
 /// </summary>
public class DeparentOnCollision : CollisionReceiver {

    public Transform deparentTo;

    public override void OnCollideYourFaction(ProxyCollision other) {
        //Camera.main.GetComponent<SimplestMovement>().enabled = false;
        SimplestMovement player = other.GetComponent<SimplestMovement>(); // might be bullet
        if (!player) {
            player = other.transform.parent.GetComponent<SimplestMovement>();
        }
        if (player) {
            player.enabled = false;
            player.transform.parent = deparentTo;
            deparentTo.GetComponent<SimplestMovement>().SetMoveLerp(0, 1);
        }
    }
}
