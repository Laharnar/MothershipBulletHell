using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class HpControlTemp : DamageCollision {
    public float maxHp;
    public float armor = 0; // how many % is damage reduced
    public float maxShields = 0; // energy shield pts

    float _hp;
    float _armor; // originalArmor
    float _shield;

    public bool destroyWhenHpIsZero = true;
    public bool destroyHitEnemies = false;
    public Action beforeDestroyed;

    public UnitInfo info;

    // just to trigger hp bar update?
    internal void UpdateHp() {
        hp = hp;
    }

    public float hp {
        get { return _hp; }
        set {
            _hp = Mathf.Clamp(value, 0, maxHp);
            HpUpdate();
            if (_hp > 0) {
                return;
            }
            HpZero();
            if (beforeDestroyed != null) {
                beforeDestroyed();
            }
            if (destroyWhenHpIsZero) {
                Destroy(gameObject);
            }
        }
    }

    public float shield {
        get {
            return _shield;
        }
        set {
            _shield = Mathf.Clamp(value, 0, maxShields);
            ShieldUpdate();
        }
    }

    protected void Start() {
        //base.Start();
        Init();
    }

    public void Init() {
        // old
        if (info && info.Get<SimpleUnit>()) {
            info.GetLast<SimpleUnit>().useMovement = true;
            info.GetLast<SimpleUnit>().useRotation = true;
        }

        hp = maxHp;
        shield = maxShields;
    }

    [System.Obsolete("Remove, check is done in hp's property")]
    protected virtual void DestroyedCheck() {
        if (hp == 0 && destroyWhenHpIsZero) {
            // old
            if (info.Get<SimpleUnit>()) {
                info.GetLast<SimpleUnit>().useMovement = false;
                info.GetLast<SimpleUnit>().useRotation = false;
            }

            Destroy(gameObject, InstancePool.PoolingMode.Move);
        }
    }

    internal void Damage(int damage, bool applyArmor = true) {
        armor = Mathf.Clamp(armor, 0, 1);

        // get actual dmg left after shields
        float shieldsLeft = shield - damage;// 4 shields, 5 dmg = 4
        float shieldsDmg = damage;
        float hpDmg = shieldsLeft < 0 ? -shieldsLeft : 0;

        shield -= shieldsDmg;
        hp -= hpDmg * (1 - _armor);
    }

    internal void Heal(int amount) {
        Damage(-amount, false);
    }


    protected virtual void HpZero() { }
    protected virtual void HpUpdate() { }

    protected virtual void ShieldUpdate() { }
}

public class GroupUnitControl : HpControlTemp {

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
