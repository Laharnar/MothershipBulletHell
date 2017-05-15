
using System.Collections;
using UnityEngine;


/// <summary>
/// Simple class to derive guns from
/// </summary>
public class GunBase : PooledMonoBehaviour {


    public string gunType;

    protected bool fire;

    public float fireRate = 0.1f;
    public Transform projectile;
    public Transform spawnPoint;

    public Spaceship source;
    internal int hits;// how many hits was done by this tower

    void Start() {
        OnStart();
    }

    protected virtual void OnStart() {
        StartCoroutine(Fire());
    }

    internal virtual void Retarget(UnityEngine.Transform target) {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// resome firing after a little pause
    /// </summary>
    /// <param name="pauseTime"></param>
    /// <returns></returns>
    protected virtual IEnumerator ResumeFire(float pauseTime) {
        fire = false;
        yield return new WaitForSeconds(pauseTime);
        fire = true;
    }

    protected virtual void FireLoopFrame(ref float time) {
        // if gun is loaded and then fire
        if (fire) {
            if (time <= Time.time) {
                FireBullet(spawnPoint);
                time = Time.time + fireRate;
            }
        }
    }
    protected virtual void FireBullet(Transform spawnPoint) {
        
        Transform gunSpawn = Instantiate(projectile, InstancePool.PoolingMode.Move) as Transform;
        gunSpawn.position = spawnPoint.position;
        gunSpawn.rotation = spawnPoint.rotation;

        var sb = gunSpawn.GetComponent<Bullet>();
        if (!sb) gunSpawn.GetComponent<BulletType>().OnInstantiate(this);// backward support
        else sb.InitBullet(null);
    }

    protected virtual IEnumerator Fire() {
        float time = Time.time;
        while (true) {

            FireLoopFrame(ref time);

            yield return null;
        }
    }


    /// <summary>
    /// Other script callback that target was destroyed
    /// </summary>
    /// <param name="destroyedTarget"></param>
    internal virtual void TargetDownCallback(Transform destroyedTarget) {
        print("Doing nothing after destroying " + destroyedTarget);
    }
}
