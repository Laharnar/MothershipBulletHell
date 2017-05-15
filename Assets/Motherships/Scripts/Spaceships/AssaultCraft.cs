using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Assault craft
/// Medium-small support craft that attack by making fast approach horizontal on target to avoid their guns or draw fire while
/// putting all the guns on 1 side in full salvo. Once out of range it makes U turn and reepats, keeping guns on same side.
/// 
/// Read SpaceshipDocs for more information.
/// 
/// Created: 14.08.2016
/// </summary>
public class AssaultCraft : Spaceship {

    // todo: replace node instances with a bunch of generated vectors that point to each other
    // move, find wp, get path of legth 2, align with path, move guns to 1 side, pass, turn, repeat
    CircularPath targetsNodes;
    QuePath travel;

    enum Pattern {
        Patrol,// unused
        Engage,
        Disengage, // goes away from target
        Rafal, // follows path horizontal to target

    }

    Pattern state = Pattern.Patrol;

    bool lockRotation;

    public Side leftSide; 
    public Side rightSide;

    public string status = "";

    bool engageBegun = false;

    #region Copy paste from bomber
    public float mobility = 25.0f;//mobility in 360 degrees per mobility seconds

    //* targeting *//
    /// <summary>
    /// Where is craft going
    /// </summary>
    Vector3 _travelPosition;
    Vector3 travelPosition { get { return _travelPosition; } set {
            if (_travelPosition != value) {
                _travelPosition = value;
                minDistToTravelPos = float.MaxValue;
                timeSinceTravelPosChange = Time.time;
            }
        } }
    // for checking flying in circle
    float timeSinceTravelPosChange;
    float minDistToTravelPos;
    float maxDistToTravelPos;

    //* movement *//
    public UpdateQueue updateQueue;

    // what was the last target that was engaged on
    Transform lastEngageTarget = null;

    bool usedAfterEngage = false;
    float disengageTime = 0;

    public DecisionMaxPositiveChoice root { get; private set; }

    int swapState = 0;

    // Use this for initialization
    void Start () {
        base.StartSpaceship();

        rigidbody2d = GetComponent<Rigidbody2D>();
        rigidbody2d.mass = 0;

        
        //target = GameObject.Find(targetName).transform;
        //targetsNodes = target.GetComponentInChildren<CircularPath>();
        state = Pattern.Engage;

        updateQueue.updateList[1] = Steering;

        travelPosition = transform.position;
        travel = new QuePath();

        //StartCoroutine(AssaultShipLogic());
        AssaultLogicInit();
    }

    
    void Update() {
        root.Do();
    }

    #endregion

    #region IMPORTANT!!!!!!!!!!!!!!!!!!
    /**
     * Travel over given path
     * Fire guns in bursts
     * Move guns from 1 side to the other
     * 
     * logic:
     * *engage: - travelpath, swap guns
     * travel on path until coming near the target
     * swap one gun on the side of target, if its same target as last rafal, 
     * *rafal: shoot
     * fire short bursts on guns while travelling on path
     * at the end of path disengage
     * *disengage: - reengage
     * move foward for a while and repeat the attack
     * */
    void AssaultLogicInit() {
        root = new DecisionMaxPositiveChoice("movement",
            new UtilityNode(Search,
                new ArgsFn(EvaluateState, 0f),
                new ArgsFn(NoTarget, 300f)),
            new UtilityNode(EngageViaPath,
                new ArgsFn(NoTarget, -30000f),
                new ArgsFn(EvaluateState, 1f),
                new ArgsFn(EvaluateState, 2f),
                new ArgsFn(EvaluateState, 3f)),
            // prevent flying in circles
            new UtilityNode(Agressive,
                new ArgsFn(NoTarget, -30000f),
                new ArgsFn(EvaluateState, 4f)),
            new UtilityNode(Disengage,
                new ArgsFn(NoTarget, -30000f),
                new ArgsFn(EvaluateState, 5f), // long disengage, set disengage tim to Time.time
                new ArgsFn(EvaluateState, 6f), // short disengage, set disengage time to Time.time-some value
                new ArgsFn(DisengageDistanceToEnemy, 18f),
                new ArgsFn(ImminentDangerHigh, 0.25f)
                ),
            new UtilityNode(Distrupted,
                new ArgsFn(EvaluateState, 7f) // Todo: set state to 7 and set disengage time
                )
        );
    }

    #region sensors

    private float EvaluateState(float s) {
        if (swapState == s) {
            return 100;
        }
        return 0;
    }

    float NoTarget(float add) {
        if (target == null) {
            return add;
        }
        return 0;
    }
    
    float DisengageDistanceToEnemy(float goToDist) {
        if (!target)
            return -30000;
        float multiplier = 20;
        return (goToDist - Vector3.Distance(transform.position, target.position));
    }

    float ImminentDangerHigh(float limitPerc) {
        if (hp.hp/hp.maxHp < limitPerc) {
            return 200;
        }
        return 0;
    }

    #endregion

    #region actions
    

    void Agressive() {
        
        status = "Attacking";
        if (!usedAfterEngage) {
            Debug.Log("repositioning guns.");
            RepositionGuns();//puts guns on other side
            usedAfterEngage = true;
        }
        

        if (travel.NodeCount() > 0) {
            GunBursts();
            Debug.DrawLine(transform.position, travelPosition, Color.red);
            if (!DistanceTooBig()) {
                travelPosition = travel.NextNode().transform.position;// when arrive to it, go to next
            }
        } else if (!DistanceTooBig()){
            // path completed
            swapState = 5;
            Debug.Log("end of path");
            engageBegun = false;
            usedAfterEngage = false;
            disengageTime = Time.time;
        }

    }

    void EngageViaPath() {
        float dist = Vector3.Distance(transform.position, travelPosition);
        if (dist < minDistToTravelPos) {
            minDistToTravelPos = dist;
        }
        if (!engageBegun) {
            // first item in path
            swapState = 2;
            CreatePath();
            travelPosition = travel.NextNode().transform.position;
            status = "engage, no fire";
            engageBegun = true;
        } else {
            // continue on path
            swapState = 3;
            if (!DistanceTooBig())
                swapState = 4;
        }
    }

    void Distrupted() {
        status = "Distrupted";
        // disengaging

        if (Time.time < disengageTime + 3) {
            travelPosition = transform.position + transform.forward * 5;
            lockRotation = true;
            DeactivateGuns();

        } else {// reset
            lockRotation = false;
            swapState = 0;
        }
    }


    void Disengage() {
        status = "Disengaging";
        // disengaging

        if (Time.time < disengageTime + 3) {
            travelPosition = transform.position + transform.forward * 5;
            lockRotation = true;
            DeactivateGuns();

        } else {// reset
            lockRotation = false;
            swapState = 0;
            Debug.Log("Disengage complete.");
        }
    }

    void Search() {
        status = "Searching";
        target = GameObject.Find(targetName).transform;
        swapState = 1;
    }

    void CreatePath() {
        if (target) {
            // todo: add advanced decisions how long to stay, 
            // -> store past path length, received damage, damage done, and chose best option 
            int pathLength = UnityEngine.Random.Range(1, 8);
            targetsNodes = target.GetComponentInChildren<CircularPath>();
            travel = targetsNodes.GetPath(targetsNodes.Closest(transform.position), pathLength);
            travel.StartOnPath();
        }
    }
    #endregion
    #endregion IMPORTANT!!!!!!!!

    private IEnumerator AssaultShipLogic() {
        Debug.Log("[obsolete]");
        // making second run on target will put all guns on 1 side
        while (true) {
            // engage(approach) -> rafal -> 
            if (state == Pattern.Engage) {
                yield return StartCoroutine(EnterPath());
                if (lastEngageTarget == target && target != null) {
                    RepositionGuns();//puts guns on other side
                }
                lastEngageTarget = target;

                state = Pattern.Rafal;
                
            } else if (state == Pattern.Rafal) {
                // fire bullets while travelling along the path
                GunBursts();
                state = Pattern.Disengage;
            } else if (state == Pattern.Disengage) {
                DeactivateGuns();
                yield return StartCoroutine(SimpleForward(5));
                state = Pattern.Engage;
            } else yield return null;
        }
    }


    private void RepositionGuns() {
        // checks on which side is be player and then move 1 gun there
        switch (GetSideOfFocus()) {
            case 1:
                // target is on left side, move from right to left side
                Side.TransferSingleGun(rightSide, leftSide, rightSide.guns.Count - 1);
                break;
            case -1:
                // target is on left side, move from right to left side
                Side.TransferSingleGun(leftSide, rightSide, leftSide.guns.Count - 1);
                break;
        }

    }

    /// <summary>
    /// finds direction of target relative to the ship
    /// </summary>
    /// <returns></returns>
    private int GetSideOfFocus() {
        var relativePoint = transform.InverseTransformPoint(target.position);
        if (relativePoint.x < 0.0) // change into < later. and change 0.0s into something else to have better forward
            return -1; // target on left
        if (relativePoint.x > 0.0) {
            return 1;  // target on right
        }
        return 0; // target directly forward
    }

    private void GunBursts() {
        int fireDirection = GetSideOfFocus();
        if (fireDirection == -1) {
            leftSide.ActivateGuns(true);
        } else {
            rightSide.ActivateGuns(true);
        }
    }

    private void DeactivateGuns() {
        leftSide.ActivateGuns(false);
        rightSide.ActivateGuns(false);
    }

    #region path commands
    private IEnumerator SimpleForward(float seconds) {
        lockRotation = true;
        Debug.DrawLine(transform.position, travelPosition, Color.blue, 5);
        yield return new WaitForSeconds(seconds);
        lockRotation = false;
    }

    /// <summary>
    /// Travel along the whole path
    /// </summary>
    /// <returns></returns>
    private IEnumerator TravelPath() {
        while (travel.NodeCount() > 0) {
            Debug.DrawLine(transform.position, travelPosition, Color.red, 5);
            yield return new WaitWhile(DistanceTooBig);
            travelPosition = travel.NextNode().transform.position;// when arrive to it, go to next
        }
    }



    /// <summary>
    /// Chose path and go near first node
    /// </summary>
    /// <returns></returns>
    private IEnumerator EnterPath() {
        travel = targetsNodes.GetPath(targetsNodes.Closest(transform.position), 2);
        QuePath bakFullPath = travel;// unused

        travelPosition = travel.NextNode().transform.position;//get first node
        Debug.DrawLine(transform.position, travelPosition, Color.blue, 5);

        yield return new WaitWhile(DistanceTooBig);
    }
    #endregion


    #region Helper functions
    private bool DistanceTooBig() {
        if (Vector3.Distance(travelPosition, transform.position) < 2f) {
            return false;
        }
        return true;
    } 
    #endregion

    void Steering() {

       if (lockRotation)
           return;

        // temporarily disabled
        /*if (sideEngine.restrictRotation) {
            mobility = mobility * 0.5f;// increase time it takes to make the turn
        }*/
        Debug.DrawLine(transform.position, travelPosition, Color.blue);

        Quaternion newRotation = Quaternion.LookRotation(transform.position - travelPosition, Vector3.forward);
        newRotation.x = 0.0f;
        newRotation.y = 0.0f;
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * mobility);

    }

    /// <summary>
    /// Fly in same direction and explode
    /// </summary>
    /// <param name="destroyedBy"></param>
    protected override void OnDeath(GunBase destroyedBy) {
        GetComponent<BoxCollider2D>().enabled = false;
        if (sideEngine)
            sideEngine.restrictRotation = true;

        base.OnDeath(destroyedBy);
    }
}

[System.Serializable]
public class Side {
    public List<AutoFireGun> guns;

    public Side() {
        guns = new List<AutoFireGun>();
    }

    public void ActivateGuns(bool active) {
        foreach (AutoFireGun gun in guns) {
            gun.AutoFireOn(active);
        }
    }

    /// <summary>
    /// Move single gun from one side to the other.
    /// </summary>
    /// <param name="thisSide"></param>
    /// <param name="newSide"></param>
    /// <param name="gunIndex"></param>
    public static void TransferSingleGun(Side thisSide, Side newSide, int gunIndex) {
        if (gunIndex == -1)
            return; // no guns on that side
        if (gunIndex < 0 || gunIndex >= thisSide.guns.Count) {
            Debug.LogError("Invalid index or empty array. ary count:"+thisSide.guns.Count + " i:"+gunIndex);
            return;
        }
        AutoFireGun g = thisSide.guns[gunIndex];
        thisSide.guns.RemoveAt(gunIndex);
        newSide.guns.Add(g);
        ((MovableGun)g).ChangeSides();
    }
}
