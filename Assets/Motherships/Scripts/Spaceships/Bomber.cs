using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bomber : Spaceship {

    //* on death *//
    public float selfDestructRadius = 1;
    private bool markDead;// safety to prevent looping explosion calls when self destructing near other bombers

    public UpdateQueue linkedUpdate;

	// Use this for initialization
	void Start () {
        base.StartSpaceship();
        UnitsInScene.RegisterUnit(transform);
        GameObject tg = GameObject.Find(targetName);
        if (tg) {
            target = tg.transform;
        } else {
            Debug.Log("Can't manualy find target "+targetName);
        }
	}
    

    #region Collision behaviour
    /// <summary>
    /// Collision handles actual collision and explosion triggering
    /// </summary>
    /// <param name="collider"></param>
    internal override void OnCollide(Spaceship otherShip)
    {
        if (this.alliance != otherShip.alliance) {
            this.SelfDestruct();
        } else {
            // self colliding is -deadly- disabled for bombers
            //base.OnCollide(otherShip);
        }
    }

    /// <summary>
    /// Handles explosion range and damage. Damage works by finding overlaping colliders.
    /// </summary>
    private void SelfDestruct() {
        Collider2D[] hitColliders = ScanArea();

        foreach (var possibleEnemy in hitColliders) {
            //foreach (var item in targeting.targets) {
            BulletCollisionHandler ship = possibleEnemy.GetComponent<BulletCollisionHandler>(); //item;
            
            if (ship && ship.source) {
                if (ship == this) {
                    continue;
                }
                if (ship.source.shipType == "Bomber") { // bombers make chain reactions to other bombers in range
                    SpreadSelfDestructToNearbyBomber(ship.source);
                } else {
                    var enemy = ship.source;
                    ApplySelfDestructDamage(enemy);
                }
            }
        }
        Destroy(gameObject);
    }

    private void SpreadSelfDestructToNearbyBomber(Spaceship ship) {
        // mark already suiciding bombers, to prevent looped self destruct sequence
        if (!markDead) {
            markDead = true;
            ship.GetComponent<Bomber>().SelfDestruct();
        }
    }

    private void ApplySelfDestructDamage(Spaceship enemy) {
        enemy.GetDamage(GetAttack(), null);
    }


    Collider2D[] ScanArea() {
        return Physics2D.OverlapCircleAll(transform.position, selfDestructRadius);
    }
    #endregion

    protected override void OnDeath(GunBase destroyedBy) {
        DisableCollision(false);
        sideEngine.restrictRotation = true;

        base.OnDeath(destroyedBy);
    }

    private void DisableCollision(bool p) {
        for (int i = 0; i < childColliders.Length; i++) {
            childColliders[i].enabled = false;
        }
    }
}
