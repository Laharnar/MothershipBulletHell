using UnityEngine;
using System.Collections;


/// <summary>
/// Recieves collisions and sends them to derived classes or to callback
/// 
// Needs rigidbody, it isn't added as requirment since CollisionReceiver derives from Proxy
// Needs trigger collider
/// </summary>
public class ProxyCollision : FilteredCollision {

    public bool useCallback = true;
    public CollisionReceiver callback;

    // use only outside proxies. this object wont have collisions. btw you can just remove collider
    public bool ignoreCollision = false;

    public bool ignoreExitCollision = false;
    public bool debugCollisions = false;

    void Awake() {
        if (ignoreCollision) {
            GetComponent<Collider2D>().enabled = false;
            Debug.Log("Remove collider if you dont need collisions instead.", this);
        } else {
            if (useCallback && !callback) {
                Debug.Log("Missing callback", this);
            }
        }
    }

    protected void OnTriggerEnter2D (Collider2D other) {
        if (ignoreCollision) return;

        //Debug.Log("enter: "+name +" collide-> " +other);

        int selectedFilter = -1;
        ProxyCollision collision = other.GetComponent<ProxyCollision>();
        if (callback && Filter(collision, out selectedFilter)) {
            callback.OnCollide(collision, selectedFilter);
        }
        if (debugCollisions) {
            Debug.Log(other + " " + transform + " "+selectedFilter, other);
        }
	}

    [System.Obsolete("move it to different script")]
    protected void OnTriggerExit2D(Collider2D other) {
        if (ignoreExitCollision) return;

        //Debug.Log("exit: "+name +" collide-> " +other);
        int selectedFilter = -1;
        ProxyCollision collision = other.GetComponent<ProxyCollision>();
        if (callback && Filter(collision, out selectedFilter))
            callback.OnCollideExit(collision, selectedFilter);
    }
}
