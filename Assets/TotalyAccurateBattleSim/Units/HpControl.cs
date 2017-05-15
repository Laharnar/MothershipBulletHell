using UnityEngine;
using System.Collections;
using System;

public partial class HpControl : DamageCollision {

    public float maxHp;
    public float armor = 0; // how many % is damage reduced
    public float maxShields = 0; // energy shield pts

    float _hp;
    float _armor; // originalArmor
    float _shield;

    public bool destroyWhenHpIsZero = true;
    public bool destroyHitEnemies = false;

    // just to trigger hp bar update?
    internal void UpdateHp()
    {
        hp = hp;
    }

    public float maxHealth { get { return maxHp; } }

    public float hp
    {
        get { return _hp; }
        set
        {
            _hp = Mathf.Clamp(value, 0, maxHealth);
            HpUpdate();
            if (_hp > 0)
            {
                return;
            }
            HpZero();
            if (destroyWhenHpIsZero)
            {
                Destroy(gameObject);
            }
        }
    }

    public float shield {
        get
        {
            return _shield;
        }
        set
        {
            _shield = Mathf.Clamp(value, 0, maxShields);
            ShieldUpdate();
        }
    }

    protected void Start()
    {
        Init();
    }

    public void Init() {
        base.Start();

        if (info && info.Get<SimpleUnit>()) {
            
            info.GetLast<SimpleUnit>().useMovement = true;
            info.GetLast<SimpleUnit>().useRotation = true;
        }
        hp = maxHp;
        shield = maxShields;
    }

    [System.Obsolete("Remove, check is done in hp's property")]
    protected virtual void DestroyedCheck()
    {
        if (hp == 0 && destroyWhenHpIsZero)
        {
            if (info.Get<SimpleUnit>()) {
                info.GetLast<SimpleUnit>().useMovement = false;
                info.GetLast<SimpleUnit>().useRotation= false;
            }
            Destroy(gameObject, InstancePool.PoolingMode.Move);
        }
    }

    internal void Damage(int damage, bool applyArmor = true)
    {
        armor = Mathf.Clamp(armor, 0, 1);

        // get actual dmg left after shields
        float shieldsLeft = shield - damage;// 4 shields, 5 dmg = 4
        float shieldsDmg = damage;
        float hpDmg = shieldsLeft < 0 ? -shieldsLeft : 0;

        shield -= shieldsDmg;
        hp -= hpDmg * (1-_armor);
    }

    internal void Heal(int amount) {
        Damage(-amount, false);
    }


    protected virtual void HpZero() { }
    protected virtual void HpUpdate() { }

    protected virtual void ShieldUpdate() { }

}
