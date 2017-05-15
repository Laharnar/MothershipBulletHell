using UnityEngine;
using System.Collections;
using System;


// this should be renamed to structure and used for guns too plus add another layer for spaceships wth movement, IF guns will have hp
public class Spaceship : PooledMonoBehaviour {

    public HpControl hp;

    [SerializeField]
    float health {

        get {
            return hp.hp;
        }
        set {
            hp.UpdateHp();
            if (OnHpUpdate != null) {
                OnHpUpdate(hp.hp);
            }
        }
    }

    public int explosionDamage;

    internal float maxHealth;
    /// <summary>
    /// functions that should call when hp is updated
    /// </summary>
    protected Action<float> OnHpUpdate;
    public Action OnHpZero;

    public DeathExplosion onDeathAnimation;

    public string targetName = "Carrierv2_Demo";

    public string alliance = "hostile";

    public int ramDamage;// damage that happens when ship hits something.

    public string shipType;

    protected Transform target;
    Vector3 _targetPosition;

    public Rigidbody2D rigidbody2d;

    public CircleCollider2D[] childColliders;
    public float refuelRange = 5;

    public LerpedSideEngine sideEngine;

    public ScrapDrop debree;

    internal bool init = false;

    protected Vector3 targetPosition {
        get {
            // if target exists, gets its position, else get last known position
            if (target)
                return _targetPosition = target.position;
            else return _targetPosition;
        }
    }

    protected void Start() {
        StartSpaceship();
    }

    protected void StartSpaceship() {
        init = true;
        maxHealth = health;

        this.InspectorNullComponentWarning(onDeathAnimation, "On Death variable is null. Requires component of type DeathExplosion. It should be on separated object as child to this.");

        if (explosionDamage == 0) {
            UnityConsole.WriteWarning("Warning: damage is zero. Source:" + name, this);
        }
        if (health == 0) {
            UnityConsole.WriteWarning("Warning: health is zero. Object might insta-destroy. Source:" + name, this);
        }

        if (OnHpUpdate == null) {
            //Debug.LogWarning("On hp updated is null. Nothing happens when hp changes, aside from reducing it's value. Source:" + transform.name, this);
        }


        this.rigidbody2d.mass = 0;
        this.rigidbody2d.gravityScale = 0;
        this.rigidbody2d.isKinematic = false;
        //this.rigidbody2d = GetComponent<Rigidbody2D>();

        //cols = GetComponentsInChildren<CircleCollider2D>();
    }

    public int GetAttack() {
        return explosionDamage;
    }

    public void GetDamage(int damage) {
        health -= damage;
        //on destroy i think should be called when hp is 0, from property hp
        //if (health <= 0) {
            //OnDeathh(destroyedBy);
        //}
    }

    public void GetDamage(int damage, GunBase destroyedBy) {
        health -= damage;
        
        if (health<=0) {
            OnDeathh(destroyedBy);
        }
    }

    private void OnDeathh(GunBase destroyedBy) {
        // fire external updates, like scrap drop

        if (OnHpZero != null) {
            OnHpZero();
        }

        // play animation, cleanup objects
        OnDeath(destroyedBy);
    }

    public void Repair(float amount){
        health += amount;
    }

    private void HandleDestroyByGun(GunBase destroyedByGun) {
        if (destroyedByGun.gunType == "Gun" || destroyedByGun.gunType == "RocketLauncher") {
            destroyedByGun.TargetDownCallback(transform);
        } else if (destroyedByGun.gunType == "AutoFireGun") {
            // this gun type doesn't use targets
        }
        else if (destroyedByGun.gunType == "MovableGun") {
            // this gun type doesn't use targets
        } else if (destroyedByGun.gunType == "") {
            Debug.LogWarning("Gun type isn't assigned to this gun. OnDestroyed callback won't work", destroyedByGun);
        }
    }

    protected virtual void OnDeath(GunBase destroyedBy) {
        debree.Drop();

        EnemyManager.ShipDown(transform);
        if (onDeathAnimation) {
            if (destroyedBy != null) {
                HandleDestroyByGun(destroyedBy);
            }
            onDeathAnimation.Begin();
        } else
            Destroy(gameObject);
    }

    private new void Destroy(UnityEngine.Object go) {
        // disable movement on "destroyed" object before pooling
        init = false;
        DisableMovement();

        Destroy(go, InstancePool.PoolingMode.Move);
    }

    void DisableMovement() {
        Debug.Log("todo:disable movement when destroyed");
    }

    /// <summary>
    /// Add action that should fire when hp changes
    /// </summary>
    /// <param name="callback"></param>
    public void RegisterShipHpStatus(Action<float> callback) {
        OnHpUpdate+= new Action<float>(callback);
    }

    /// <summary>
    /// Function that should fire when ship is out.
    /// </summary>
    /// <param name="callback"></param>
    public void RegisterOnDeath(Action callback) {
        if(onDeathAnimation)
            onDeathAnimation.explosionTrigger += callback;
    }

    public void RegistedOnHpZero(Action callback) {
        OnHpZero += callback;
    }

    /// <summary>
    /// What should happen once new target is found.
    /// 
    /// For now TargetTracking script activates it, using 2d circle trigger collider.
    /// 
    /// Date: 3.7.2016
    /// </summary>
    /// <param name="carrier"></param>
    internal virtual void TargetReceived(Carrier carrier) {
        if (target == null ||
            Vector3.Distance(carrier.transform.position, transform.position) < Vector3.Distance(target.position, transform.position)
            ) {
            target = carrier.transform;
        }
        //throw new NotImplementedException("Implement this function on derived classes.");
    }

    /// <summary>
    /// Basic bullet handling handles just standard bullets that aren't of same side.
    /// </summary>
    /// <param name="bullet"></param>
    internal virtual bool DamageFromBullet(BulletType bullet) {
        // hit by player's bullet
        bool appliedDmg = false;
        if (bullet.alliance != alliance) {
            if (bullet.origin)
                GetDamage(bullet.damage);
            else // spaceship version
                GetDamage(bullet.damage, bullet.gunOrigin);// damages and resets gun target, which should be done by behaviour
            appliedDmg = true;
        }
        return appliedDmg;
    }

    /// <summary>
    /// Handles on ship collisions
    /// </summary>
    /// <param name="collider"></param>
    void OnCollisionEnter2D(Collision2D collider) {
        Spaceship ship = collider.gameObject.GetComponent<Spaceship>();
        if (ship) {
            OnCollide(ship);
        } else {
            if (collider.gameObject.name.Contains("Bomber")) {
                Debug.Log("Type error, requres spaceship");

            }
        }
    }

    /// <summary>
    /// Handles on ship collisions
    /// </summary>
    /// <param name="collider"></param>
    void OnTriggerEnter2D(Collider2D collider) {
        Spaceship ship = collider.gameObject.GetComponent<Spaceship>();
        if (ship) {
            OnTrigger(ship);
        }
    }

    internal virtual void OnTrigger(Spaceship otherShip) {
        OnCollide(otherShip);
    }

    internal virtual void OnCollide(Spaceship otherShip) {
        // by default ramming other ships applies ram damage, and half ram damage to itself. applies all spaceships, including to friendlies
        this.GetDamage((int)(ramDamage / 2), null);
        otherShip.GetComponent<Spaceship>().GetDamage(ramDamage, null);
    }

}
