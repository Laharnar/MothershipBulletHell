using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Has function for setting aiming and shooting at targets or locations.
/// -----------------
/// Handles player and ai behvaiour for guns.
/// 
/// 
/// auto aim - aim at newest target
/// manual aim - aim at mouse and shoot turn on shooting with space, control locks rotation
/// directional aim - aim at point, activated by right click from any mode
/// target aim - aim at target if in range, else just in that direction, activated by right click on enemy from any mode
/// </summary>
[RequireComponent(typeof(SceneDependantAI))]
public class SimpleGun : GunsAbstract {
    
    /*
    public UnitsInScene scene;
    public SceneDependantAI tracking;
    public ShipInfo source;

    bool holdFire = true;
    bool reloading = false;

    public float fireRate = 0.1f;
    public float fireRateUpdate = 0.05f;
    public Transform projectile;

    public string spawnPointChildName = "";
    public Transform spawnPoint;

    Vector2 aimAt = Vector2.zero;
    //Vector2 newAim = Vector2.zero;
    //float t;

    internal int hitCount;

    Transform target;
    internal bool devDisableFire = false;

    public TimeCount firing;
    public TimeCount aiming;

    public DecisionMaxPositiveChoice fireLogic;

    // Use this for initialization
    void Awake () {
        if (spawnPointChildName != "")
            spawnPoint = transform.FindChild(spawnPointChildName);

        if (scene == null) {
            scene = GameObject.FindObjectOfType<UnitsInScene>();
        }
        if (tracking == null) {
            tracking = GetComponent<SceneDependantAI>();
            Logger.AddLog("tmpFix-Assign tracking script.", this);
        }

        firing.Init(fireRate);
        aiming.Init(fireRateUpdate);
        //StartCoroutine(AimAtRndPoints());
        //StartCoroutine(FireLoop());

        /** gun logic
         * if no target, hold fire
         * if no hold fire, every x seconds update aiming direction to targets
         * rotate towards aim point
         * */
    /*  fireLogic = new DecisionMaxPositiveChoice("gun logic",
          new UtilityNode("fire if target", FireAtTarget, new ArgsFn(HasTarget)),
          new UtilityNode("hold fire", HoldFire, new ArgsFn(NoTarget)));


  }

  void Update() {

      /*if (Time.time > t && !holdFire) {
          t = Time.time + fireRateUpdate;
          aimAt = target.position;//tracking.AimAtRandom();
      }*/

    /*     fireLogic.Do();

         // rotation update
         if (aimAt != Vector2.zero)
             transform.TurnTowards(new AiInfo(tracking), aimAt);
         Debug.DrawLine(transform.position, aimAt, Color.yellow);
     }

     void FireAtTarget() {
         FireGun();

         // re-aim at target
         if (aiming.IsTime() && !holdFire) {
             aiming.Next();
             aimAt = target.position;//tracking.AimAtRandom();
         }
     }

     float NoTarget() {
         if (target == null || devDisableFire) {
             return 1;
         }
         return -1;
     }

     float HasTarget() {
         if (target == null || devDisableFire) {
             return -1;
         }
         return 1;
     }

     public void SetTarget(Transform target) {
         this.target = target;
         Fire();
         //StartCoroutine(ResumeFire(0.05f));// iif you use paus after target changes, make sure target doesnt change often
     }

     public void Fire() {
         if (devDisableFire) return;
         holdFire = false;
     }

     public void HoldFire() {
         holdFire = true;
     }

     private IEnumerator AimAtRndPoints()
     {
         float t = Time.time + 1;
         Vector3 aimAt = new Vector3(); 

         while (true)
         {
             if (Time.time > t)
             {
                 t = Time.time + 1;
                 aimAt = tracking.AimAtRandom();
             }
             transform.TurnTowards(tracking, aimAt);
             yield return null;
         }
     }


     /// <summary>
     /// resume firing after a little pause
     /// </summary>
     /// <param name="pauseTime"></param>
     /// <returns></returns>
     private IEnumerator ResumeFire(float pauseTime) {
         HoldFire();
         yield return new WaitForSeconds(pauseTime);
         if (target)
             Fire();
     }

     private void FireGun() {
         if (firing.IsTime()) {
             reloading = false;
         }
         if (!holdFire && !reloading) {
             reloading = true;
             FireBullet(spawnPoint);
         }
     }

     private void FireBullet(Transform spawnPoint) {
         Transform gunSpawn = Instantiate(projectile, InstancePool.PoolingMode.Move) as Transform;
         gunSpawn.position = spawnPoint.position;
         gunSpawn.rotation = spawnPoint.rotation;
         gunSpawn.GetComponent<Bullet>().InitBullet(source.flag.alliance);
     }*/

}

/// <summary>
/// Simple timer class
/// </summary>
[Serializable]
public class TimeCount {
    float t;
    internal float rate;

    public void Init(float rate) {
        this.rate = rate;
    }

    public void Next() {
        this.t = Time.time+rate;
    }

    public void NextAt(float time) {
        t = time;
    }

    public bool IsTime() {
        return Time.time > t;
    }
}

[Serializable]
public class FireStats {
    public float fireRate = 0.1f;
    public float fireRateUpdate = 0.05f;
    internal Vector2 aimAt;
}

/// <summary>
/// Should be public
/// </summary>
[Serializable]
public class Targeting {

    /// <summary>
    /// Added 14.5.2017
    /// </summary>
    string alliance;

    internal UnitInfo target;
    public TargetingType targeting = TargetingType.Normal;

    public bool CheckTarget() {
        
        return target != null;
    }

    public Targeting(string alliance) {
        this.alliance = alliance;
    }

    internal bool ScanArea(UnitInfo info, int range) {
        //target = SceneSearching.ChoseClosestTarget(transform, alliance);
        target = SearchManager.ClosestEnemyUnit(info, alliance);
        return target != null;
    }
}

public enum TargetingType {
    Normal, // shoot at nearest
    Group // shoot at the one provided by group control
}

[Serializable]
public class ShootAbstract{

    Transform transform;
    public Targeting targeting;

    public TimeCount firing;
    public TimeCount aiming;

    bool holdFire = true;
    bool reloading = false;

    public Transform projectile;
    public SpawnPoint2 sp;

    public FireStats data;
    public string alliance;

    internal void Init(Transform transform) {
        this.transform = transform;
        firing.Init(data.fireRate);
        aiming.Init(data.fireRateUpdate);
        sp.Init(transform);
    }

    internal void FireAtTarget() {
        FireGun();

        // re-aim at target
        if (aiming.IsTime() && !holdFire) {
            aiming.Next();
            data.aimAt = targeting.target.transform.position;
        }
    }

    public float NoTarget() {
        if (!targeting.CheckTarget()) {
            return 1;
        }
        return -1;
    }

    public float HasTarget() {
        return -NoTarget();
    }

    public void SetTarget(Transform target) {
        // todo: if you need to set target, instantiate empty with unit info and parent it to target
        if (target == null) return;
        this.targeting.target = target.GetComponent<UnitInfo>();// 14.05.2017
        Fire();
        //StartCoroutine(ResumeFire(0.05f));// iif you use paus after target changes, make sure target doesnt change often
    }

    public void Fire() {
        holdFire = false;
    }

    public void HoldFire() {
        holdFire = true;
    }

    public void Search() {
        if (targeting.targeting == TargetingType.Normal) {
            Debug.Log("seaching for target ");
            SetTarget(UnitsInScene.GetClosestEnemyShip(transform, 1));
        }
    }

    private void FireBullet(Transform spawnPoint) {
        if (projectile == null) Debug.Log("Pref is null "+ projectile, transform);
        Transform gunSpawn = InstancePool.CreateUnit(projectile, InstancePool.PoolingMode.Move) as Transform;
        gunSpawn.position = spawnPoint.position;
        gunSpawn.rotation = spawnPoint.rotation;
        gunSpawn.GetComponent<Bullet>().InitBullet(alliance);
    }

    private void FireGun() {
        if (firing.IsTime()) {
            reloading = false;
        }
        if (!holdFire && !reloading) {
            reloading = true;
            FireBullet(sp.spawnPoint);
            firing.Next();
        }
    }

}

[Serializable]
public class SpawnPoint2 {

    public string spawnPointChildName;
    public Transform spawnPoint;

    public void Init(Transform transform) {
        if (spawnPointChildName != "")
            spawnPoint = transform.FindChild(spawnPointChildName);
    }
}

public class GunsAbstract: MonoBehaviour{

   
    public AiInfo data;

    DecisionMaxPositiveChoice fireLogic;
    public ShootAbstract shootingLogic;
    
    void Init() {
        shootingLogic.Init(transform);

        /** gun logic
         * if no target, hold fire
         * if no hold fire, every x seconds update aiming direction to targets
         * rotate towards aim point
         * */
        fireLogic = new DecisionMaxPositiveChoice("gun logic",
            new UtilityNode("fire if target", shootingLogic.FireAtTarget, new ArgsFn(shootingLogic.HasTarget)),
            new UtilityNode("hold fire", shootingLogic.Search, new ArgsFn(shootingLogic.NoTarget)));
    }

    void Awake() {
        Init();
    }

    void Update() {
        fireLogic.Do();

        // rotation update
        if (shootingLogic.data.aimAt != Vector2.zero) {
            transform.TurnTowards(data, shootingLogic.data.aimAt);
        }
        Debug.DrawLine(transform.position, shootingLogic.data.aimAt, Color.yellow);
    }

    public object GetLogic() {
        return fireLogic;
    }
}

/// <summary>
/// Info class for transfering data
/// </summary>
[System.Serializable]
public class AiInfo {

    public SceneDependantAI ai;

    public float rotationAccuracy;// to what angle shoul be regular rotation speed
    public float innerAccuracy; // to what angle should be slow speed
    public float nonModifiedSteering; // regular speed
    public float slowDownEffect; // slow speed

    public SimpleUnit source;

    public AiInfo(SceneDependantAI ai) {
        this.ai = ai;
    }

    public void Get() {
        rotationAccuracy = ai.rotationAccuracy;
        innerAccuracy = ai.innerAccuracy;
        nonModifiedSteering = ai.getSource.nonModifiedSteering; 
        slowDownEffect = ai.slowDownEffect;
        this.source = ai.getSource;

    }
}