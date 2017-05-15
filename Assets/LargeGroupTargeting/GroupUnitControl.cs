using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroupUnitControl : HpControl {

    public GroupControl central;
    public bool msgCentralOnDestroyed = false;

    // which objects can get managed by the central
    public MonoBehaviour[] managed;

    protected new void Awake() {
        base.Awake();

        if (central)
            central.Register(this);
    }

    protected override void HpZero() {
        if (central && msgCentralOnDestroyed)
            central.OnMemberDestroyed(this);

        base.HpZero();
    }

    public override void OnCollideEnemyFaction(ProxyCollision other) {
        base.OnCollideEnemyFaction(other);
    } 
}
