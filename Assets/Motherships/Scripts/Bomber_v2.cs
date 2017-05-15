using UnityEngine;
using System.Collections;

public class OverrideTravel {
    internal Transform target;

    bool _applySecondaryTravel;
    internal bool applySecondaryTravel {
        get {
            if (!leader) {
                _applySecondaryTravel = false;
                FormationManager.scene.RemoveUnit(source, source.isLeader);
                FormationManager.scene.AddUnit(source, source.isLeader);
            }
            return _applySecondaryTravel;
        }

        set {
            _applySecondaryTravel = value;
        }
    }
    internal Vector3 secondaryTravel;

    internal AutomaticGrouping source;
    internal AutomaticGrouping leader;

    public OverrideTravel(AutomaticGrouping source) {
        this.source = source;
    }
}
public class Bomber_v2 : MonoBehaviour {

    //* on death *//
    public float selfDestructRadius = 1;
    private bool markDead;// safety to prevent looping explosion calls when self destructing near other bombers
    public int explosionDamage;

    [System.Obsolete("Replace it with travelTo.target")]
    internal Transform target { get { return travelTo.target; } private set { travelTo.target = value; } }
    internal OverrideTravel travelTo;

    public AutomaticGrouping grouping;
    public Alliance flag;
    public SimpleUnit unit;
    public ShipInfo info;

    // temporary solution to group travelling. disables settin of waypoints by this script
    
    void Start()
    {
        travelTo = new OverrideTravel(grouping);
        GetComponent<Rigidbody2D>().gravityScale = 0;
        GetComponent<Rigidbody2D>().isKinematic = false;
        
        AquireTarget();
    }

    void Update()
    {
        if (target == null)
        {
            AquireTarget();
        }
    }

    void AquireTarget()
    {
        target = SceneSearching.ChoseClosestTarget(transform, flag.alliance);
        //add override version 
        StartCoroutine(SceneSearching.FollowTarget_v2(transform, unit, target, travelTo, false));
    }
    

    void OnTriggerEnter2D(Collider2D other)
    {
        // hitting enemy will trigger self destroy
        if (other.transform == transform)
        {
            Debug.LogError("fsaferhehsgergsdfsf.  !!!");
        }

        Spaceship sp = other.GetComponent<Spaceship>();
        Alliance oflag = other.GetComponent<Alliance>();

        if ((sp && flag.alliance != sp.alliance) || (oflag && flag.alliance != oflag.alliance))
        {
            SelfDestruct();
        }
    }

    /// <summary>
    /// Handles explosion range and damage. Damage works by finding overlaping colliders.
    /// </summary>
    private void SelfDestruct()
    {
        Collider2D[] hitColliders = SceneSearching.ScanArea(transform.position, explosionDamage);
        
        foreach (var possibleEnemy in hitColliders)
        {
            //foreach (var item in targeting.targets) {
            ShipInfo info = possibleEnemy.GetComponent<ShipInfo>(); //item;
            if (info)
            {
                if (info.type == "Bomber")
                {
                    SpreadSelfDestructToNearbyBomber(info.transform);
                } else
                {
                    ApplySelfDestructDamage(info);
                }
                continue;
            }

            BulletCollisionHandler bltOld = possibleEnemy.GetComponent<BulletCollisionHandler>(); //item;

            if (bltOld && bltOld.source)
            {
                if (bltOld == this)
                {
                    continue;
                }
                Debug.Log("TODO: Put ShipType on this object and assign type to it.", possibleEnemy);

                if (bltOld.source.shipType == "Bomber")
                { // bombers make chain reactions to other bombers in range
                    SpreadSelfDestructToNearbyBomber(bltOld.source.transform);
                } else
                {
                    var enemy = bltOld.source;
                    Debug.Log("damaging "+enemy);
                    ApplySelfDestructDamage(enemy);
                }
            }
        }
        
    }

    private void SpreadSelfDestructToNearbyBomber(Transform ship)
    {
        // mark already suiciding bombers, to prevent looped self destruct sequence
        if (!markDead)
        {
            markDead = true;
            ship.GetComponent<Bomber_v2>().SelfDestruct();
        }
    }

    private void ApplySelfDestructDamage(Spaceship enemy)
    {
        enemy.GetDamage(explosionDamage, null);
    }

    private void ApplySelfDestructDamage(ShipInfo enemy)
    {
        enemy.hp.Damage(explosionDamage);
    }
}
