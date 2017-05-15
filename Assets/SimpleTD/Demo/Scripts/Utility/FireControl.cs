using UnityEngine;
using System.Collections;

public class FireControl : MonoBehaviour {

    public ProjectileSpawner spawner;
    public TargetControl targeting;
    public AimControl aim;

    [SerializeField]
    bool fire = false;
    public float fireRate;

	// Use this for initialization
	void Start () {
        StartCoroutine(FireLoop());
	}

    private IEnumerator FireLoop() {
        while (true) {
            // case when target is destroyed or not found
            if (!aim.target) {
                aim.SetTarget(targeting.RequestNewTarget());
            }
            if (fire)
            {

                if (aim.target)
                {
                    Projectile projectile = spawner.Spawn();
                    projectile.OnInstantiated(aim.target.position);

                    yield return new WaitForSeconds(fireRate);
                }
            }
                
           

            yield return null;
        }
    }

    internal void SetNewTarget(Transform target) {
        if (target) {
            fire = true;
            aim.SetTarget( target);
        }
        else
        {
            fire = false;
            aim.SetTarget(null);
        }
    }

    internal void OnZeroTargets() {
        fire = false;
    }

    [System.Obsolete()]
    internal void OnTargetExit(Transform target)
    {
        if (aim.target == target && target != null)
        {
            aim.target=targeting.RequestNewTarget();
        }
    }
}
