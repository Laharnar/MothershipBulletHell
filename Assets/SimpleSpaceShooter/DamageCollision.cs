using UnityEngine;
using System.Collections;

/// <summary>
/// Put this script on things that apply damage on collision
/// </summary>
public class DamageCollision : CollisionReceiver {

    public int heal = 0;
    public int damage = 1;

    public bool destroyOnAllyCollision = false;
    public bool destroyOnEnemyCollision = false;
    public bool destroyOnUtilityCollision = false;
    public bool destroyOnProjectileCollision = false;

    public bool intentionalNoInfo = false; // for destroying bullets that go out of range
    public UnitInfo info;

    protected void Start() {
        if (info == null)
            Logger.AddLog("Set info.", this);
    }

    public override void OnCollideAlliedFaction(ProxyCollision other) {
        base.OnCollideAlliedFaction(other);

        DamageCollision applyTo = (DamageCollision)other.callback;
        if (applyTo.info) {
            if (applyTo.info.Get<HpControl>()) {
                applyTo.info.GetLast<HpControl>().Heal(heal);
                if (applyTo.info.GetLast<HpControl>().destroyHitEnemies) {
                    Destroy(gameObject);
                }
            }
        } else if (!applyTo.intentionalNoInfo) Debug.Log("No info is assigned.");

        if (destroyOnAllyCollision)
            Destroy(gameObject);
    }

    public override void OnCollideEnemyFaction(ProxyCollision other) {
        base.OnCollideEnemyFaction(other);

        DamageCollision applyTo = (DamageCollision)other.callback;
        if (other.useCallback)
        if ( applyTo.info) {
            if (applyTo.info.Get<HpControl>(typeof(GroupUnitControl))) {
                applyTo.info.GetLast<HpControl>().Damage(damage);
            }
        } else if (!applyTo.intentionalNoInfo) Debug.Log("No info is assigned.", applyTo);

        if (destroyOnEnemyCollision) Destroy(gameObject);
    }

    public override void OnCollideProjectile(ProxyCollision other) {
        base.OnCollideProjectile(other);
        DamageCollision applyTo = (DamageCollision)other.callback;
        if ( applyTo.info) {
            // in case bullet has hp
            if (applyTo.info.Get<HpControl>(typeof(GroupUnitControl))) {
                applyTo.info.GetLast<HpControl>().Damage(damage);
            }
        } else if (!applyTo.intentionalNoInfo) Debug.Log("No info is assigned.", applyTo);
        if (destroyOnProjectileCollision) Destroy(gameObject);
    }

    public override void OnCollideUtility(ProxyCollision other) {
        base.OnCollideUtility(other);

        if (destroyOnUtilityCollision)
            Destroy(gameObject);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj">simulate destroying by losing hp</param>
    protected void OnDestroy() {
        if (info.Get<HpControl>(typeof(GroupUnitControl)))
            info.GetLast<HpControl>().Damage(int.MaxValue);
    }
}
