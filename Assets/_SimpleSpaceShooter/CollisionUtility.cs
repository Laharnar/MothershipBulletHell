using UnityEngine;
using System.Collections;

public class CollisionUtility : CollisionReceiver {

    // Update is called once per frame
    public virtual void Activate() { }

    public override void OnCollideUtility(ProxyCollision other) {
        base.OnCollideUtility(other);

        Activate();
    }
}
