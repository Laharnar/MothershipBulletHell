using UnityEngine;
using System.Collections;

public class Rocket : AcceleratingProjectile {

    [System.Obsolete("not referenced")]
    public BurstEngine rocketMovement;

    public float radius = 1f;

    public LayerMask collisionLayer;

    protected override void OnCollisionDamage(BulletCollisionHandler hitSpaceship) {

        Collider2D[] scan = Physics2D.OverlapCircleAll(transform.position, radius, collisionLayer);
        bool destroyFlag = false; 
        
        for (int i = 0; i < scan.Length; i++) {
            BulletCollisionHandler ship = scan[i].GetComponent<BulletCollisionHandler>();
            
            if (ship) {// just a safe, layer specification should guarrante spaceship
                if (ship.source.DamageFromBullet(this)) {
                    destroyFlag = true;
                    Debug.DrawRay(transform.position - Vector3.up * radius, Vector3.up * 2 * radius, Color.red, 0.5f);
                    Debug.DrawRay(transform.position - Vector3.right * radius, Vector3.right * 2 * radius, Color.red, 0.5f);
                }
            }
        }

        if (destroyFlag) {
            DestroyBullet();
        }
    }
}
