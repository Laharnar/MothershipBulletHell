using UnityEngine;
using System.Collections;

/// <summary>
/// When called for, fires explosion animation and when it ends, destroys itself.
/// 
/// ** Requred Animator Triggers
/// Explode: Trigger
/// 
/// </summary>
public class DeathExplosion : MonoBehaviour {

    public Animator anim;
    public Spaceship origin;// put somewhere near spaceships

    /// <summary>
    /// External calls that should happen when destroyed
    /// </summary>
    internal System.Action explosionTrigger;


    /// <summary>
    /// Call from spaceships when they explode.
    /// </summary>
    /// <returns></returns>
    internal void Begin() {

        anim.SetTrigger("Explode");
    }

    /// <summary>
    /// Called from animation's frame, when origin should be destroyed and scrap spawns
    /// </summary>
    public void DestroyOrigin() {
        //Scraper.MakeScrap(origin);
        transform.parent = null;// separate axplosion and origin, so origin can be safely destroyed

        if (explosionTrigger != null) {
            explosionTrigger();
        }

        Destroy(origin.gameObject);
    }

    /// <summary>
    /// Called from animation's last frame
    /// </summary>
    public void AnimEnd() {
        Destroy(gameObject);
    }
}
