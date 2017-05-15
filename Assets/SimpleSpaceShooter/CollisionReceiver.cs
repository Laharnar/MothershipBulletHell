using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// main collision event trigger
/// </summary>
public class CollisionReceiver : PooledMonoBehaviour{

    public bool storePastCollisions = false;
    protected List<ProxyCollision> pastCollisions = new List<ProxyCollision>();

    public bool storeCurrentCollisions = false;
    List<ProxyCollision> currentCollisions = new List<ProxyCollision>(); // useful for distance calculations?

    Action<ProxyCollision>[] groups;

    protected void Awake() {
        groups = new Action<ProxyCollision>[5] {
            OnCollideYourFaction,
            OnCollideAlliedFaction,
            OnCollideUtility,
            OnCollideProjectile,
            OnCollideEnemyFaction,
        };
    }

    internal void OnCollide(ProxyCollision other, int selectedGroup) {
        // NOT collision receiver dependant

        // call on collision event before storing it
        groups[selectedGroup](other);

        if (storePastCollisions) {
            pastCollisions.Add(other);// if this throws error, either collision shouldnt happen, or you forgot to set callback
        }

        if (storeCurrentCollisions) {
            currentCollisions.Add(other);// if this throws error, either collision shouldnt happen, or you forgot to set callback
        }
    }

    internal void OnCollideExit(ProxyCollision other, int selectedFilter) {
        // TODO: use selected filter somehow?
        if (storeCurrentCollisions) {
            // objects can get destroyed over time, so check if it exists
            if (currentCollisions.Contains(other))
                currentCollisions.Remove(other);// if this throws error, either collision shouldnt happen, or you forgot to set callback
        }
    }

    public virtual void OnCollideYourFaction(ProxyCollision other) { }

    public virtual void OnCollideAlliedFaction(ProxyCollision other) { }

    public virtual void OnCollideEnemyFaction(ProxyCollision other) {
        //if (other.staticObject && !staticObject) other.OnCollideEnemyFaction((DamageCollision)this);
    }

    public virtual void OnCollideProjectile(ProxyCollision other) {
        //if (other.staticObject && !staticObject) other.OnCollideEnemyFaction((DamageCollision)this);
    }

    public virtual void OnCollideUtility(ProxyCollision other) {
        //if (other.staticObject && !staticObject) other.OnCollideUtility(this);
        Debug.Log("utility " + name + " " + other);
        //CollisionUtility cu = other.GetComponent<CollisionUtility>();
        //if (cu) cu.Activate();
    }

}
