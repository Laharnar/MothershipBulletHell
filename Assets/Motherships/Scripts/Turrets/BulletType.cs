using UnityEngine;
using System.Collections;
using System;

public class BulletType : PooledMonoBehaviour {

    public string alliance = "player"; // types: player, hostile
    public int damage = 1;
    
    public float bulletLife = 5;

    [Obsolete("totaly useless, just for counting hits")]
    internal GunBase gunOrigin;// origin to gun should be manualy assigned on instantiate, to allow gun to remove it's aim from the ship.
    SimpleGun gunOwner;

    internal Transform origin;// who spawned the bullet. GunBase type is old verison

    public Rigidbody2D rig2d;

    public bool noMovement= false;

    float sTime;
    void Start() {
        Init();
    }

    void Init() {
        sTime = Time.time;
    }

    private void OnEnable() {
        Init();
    }

    void Update() {
        if (Time.time > bulletLife)
            Destroy(gameObject, bulletLife);
    }

    internal virtual void OnInstantiate(GunBase gunOrigin) {
        this.gunOrigin = gunOrigin;// this gun origin is object because AutofireGun and gun dont start from same type

        if (rig2d.gravityScale != 0) {
            UnityConsole.WriteWarning("Set rigidbody gravity scale to zero.");
        }
    }

    public virtual void DestroyBullet() {
        if (gunOrigin) gunOrigin.hits += 1; else Debug.LogWarning("no hit counter for the gun");
        Debug.Log("old "+name);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other) {
        BulletCollision(other.transform);
    }

    void OnCollisionEnter2D(Collision2D other) {
        BulletCollision(other.transform);
    }

    protected virtual void BulletCollision(Transform other) {
        ShipInfoProxy ship = other.GetComponent<ShipInfoProxy>();
        if (ship)
        {
            ship.source.hp.Damage(damage);
            DestroyBullet();
        } else {
            Debug.Log("old collision, replace it with hp control and ship info", transform);
            // try to apply damage to ship bullet collided with and then destroy bullet
            BulletCollisionHandler hitSpaceship = other.transform.GetComponent<BulletCollisionHandler>();
            if (hitSpaceship)
            {
                OnCollisionDamage(hitSpaceship);
            }
        }
    }

    protected virtual void OnCollisionDamage(BulletCollisionHandler hitSpaceship) {
        if (hitSpaceship.ApplyDamageFromHostileBullets(this)) {
            DestroyBullet();
        }
    }

    internal void OnInstantiate(Transform transform) {
        origin = transform;
    }
}

