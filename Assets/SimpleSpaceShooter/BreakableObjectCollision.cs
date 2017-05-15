using UnityEngine;
using System.Collections;
using System;

public class BreakableObjectCollision : CollisionReceiver {

    public Transform builtWall;
    public Transform destroyedWall;

    public bool wallDestroyed = false;

    protected new void Awake() {
        base.Awake();
        if (!builtWall) {
            builtWall = transform;
        }
        SetWallVisibility();
    }

    public override void OnCollideEnemyFaction(ProxyCollision other) {
        base.OnCollideEnemyFaction(other);
        wallDestroyed = !wallDestroyed;
        SetWallVisibility();
    }

    private void SetWallVisibility() {
        // break a piece away
        if (wallDestroyed) {
            builtWall.GetComponent<BoxCollider2D>().enabled = false;
            builtWall.GetComponent<SpriteRenderer>().enabled = false;
            destroyedWall.GetComponent<SpriteRenderer>().enabled = true;
        } else {
            builtWall.GetComponent<BoxCollider2D>().enabled = true;
            builtWall.GetComponent<SpriteRenderer>().enabled = true;
            destroyedWall.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
