using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// Gives access to applying damage from bullets.
/// 
/// Put this script on all spaceships childs that have to intercept bullets.
/// 
/// </summary>
public class BulletCollisionHandler : MonoBehaviour {

    public Spaceship source;

    void OnTriggerEnter2D(Collider2D other) {
        HandleBullets(other.transform);
        
        // handle ram in other ships
        BulletCollisionHandler handler = other.transform.GetComponent<BulletCollisionHandler>();
        if (handler && handler != this) {
            source.OnTrigger(handler.source);
        }
    }

    void OnCollisionEnter2D(Collision2D other) {
        HandleBullets(other.transform);

        BulletCollisionHandler handler = other.transform.GetComponent<BulletCollisionHandler>();
        if (handler && handler != this) {
            source.OnCollide(handler.source);
        }
    }

    private void HandleBullets(Transform other) {
        BulletType bullet = transform.GetComponent<BulletType>();
        if (bullet) {
            ApplyDamageFromHostileBullets(bullet);
        }
    }

    internal bool ApplyDamageFromHostileBullets(BulletType bulletType) {
        bool applied = source.DamageFromBullet(bulletType);
        return applied;
    }
}