using UnityEngine;
using System.Collections;

public class LaserBullet : LinearProjectile {

    Transform laserDirection; // rotation of spawn point

    internal override void OnInstantiate(GunBase gunOrigin) {
        base.OnInstantiate(gunOrigin);

        Init(transform.position, ((AimingGunBase)gunOrigin).activeTarget.position);
    }

    public void Init(Vector3 spawnPoint, Vector3 targetPos) {
        RaycastHit2D raycast2d = Physics2D.Raycast(spawnPoint, Vector2.up, Mathf.Infinity, 1 << LayerMask.NameToLayer("LaserDetection"));
        

        if (raycast2d) {
            float y = Vector3.Distance(spawnPoint, raycast2d.point);
            transform.localScale = new Vector3(transform.localScale.x, y, transform.localScale.z);

        } else {
            float y = Vector3.Distance(spawnPoint, targetPos);
            transform.localScale = new Vector3(transform.localScale.x, y, transform.localScale.z);
        }

        speed = 0;
        noMovement = true;
    }
}
