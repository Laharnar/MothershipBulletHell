using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AssaultCraft_v2 : MonoBehaviour {

    // move, find wp, get path of legth 2, align with path, move guns to 1 side, pass, turn, repeat
    //CircularPath targetsNodes; // implement by flying left wiht target in center
    //Path travel; /// implement with array of vectors
    
    Queue<Vector3> pathAroundTarget = new Queue<Vector3>();

    Pattern state = Pattern.Patrol;
    enum Pattern
    {
        Patrol,// unused
        Engage, // fly towards target
        Circle, // rotate around target
        Disengage, // goes away from target
        Rafal, // follows path horizontal to target
    }
    public Side leftSideGuns;
    public Side rightSideGuns;

    public SimpleUnit unit;
    public ShipInfo info;

    // how far from tagte will path be first marked
    public float keepDistance = 8;

    Transform target;
    Vector3 travelTo;

    bool loopRunning = false;

    #region Copy paste from bomber
    // Use this for initialization
    void Start()
    {
        GetComponent<Rigidbody2D>().gravityScale = 0;
        GetComponent<Rigidbody2D>().isKinematic = false;

        
    }

    void Update()
    {
        

        if (target == null)
        {
            if (!loopRunning)
                StartCoroutine(StateLoop());

            AquireTarget();
        }
    }

    bool AquireTarget()
    {
        target = SceneSearching.ChoseClosestTarget(transform, info.flag.alliance);

        return target != null;
        /*if (target)
        {
            state = Pattern.Engage;

            StartCoroutine(SceneSearching.FollowTarget(transform, unit, target, false));
        }*/
    }

    #endregion

    private IEnumerator StateLoop()
    {
        loopRunning = true;

        // attacking target multiple times moves guns to the attacking side
        Transform lastEngageTarget = null;
        if (AquireTarget())
            pathAroundTarget = transform.CreateCircularPathAround(target, 8, keepDistance);
        while (true)
        {
            

            if (state == Pattern.Patrol)
            {
                // serach for target every 2 seconds
                if (!target && !AquireTarget()) {
                    yield return new WaitForSeconds(2);
                    continue;
                }
                else // found target
                state = Pattern.Engage;
            }
            // engage(approach) -> rafal -> 
            if (state == Pattern.Engage)
            {
                if (lastEngageTarget == target)
                {
                    RearmGuns();//puts guns on other side
                }
                lastEngageTarget = target;

                if (pathAroundTarget.Count > 0) {
                    Vector3 point = pathAroundTarget.Dequeue();
                    Debug.DrawLine(transform.position, point, Color.yellow, 5);
                    yield return StartCoroutine(SceneSearching.MoveToPos(transform, unit, point, false, 5, nearbyRange: 5));
                    state = Pattern.Circle;
                } else {
                    state = Pattern.Patrol;
                }
            }
            if (state == Pattern.Circle)
            {
                // fire bullets while travelling along the path
                pathAroundTarget = transform.CreateCircularPathAround(target, 16, Vector3.Distance(transform.position, target.position));
                ActivateGunBursts(true);
                yield return StartCoroutine(FlyPath());
                state = Pattern.Disengage;
            }
            if (state == Pattern.Disengage)
            {
                ActivateGunBursts(false);
                if (!target)
                {
                    unit.Idle();
                    yield return StartCoroutine(MovementCommands.SimpleTimedMove(unit, SimpleUnit.Moves.forward, 1, true));
                }
                state = Pattern.Patrol;
            }
            yield return null;
        }
    }

    private void RearmGuns()
    {
        // checks on which side is be player and then move 1 gun there
        int getSideOfAim = GetSideOfFocus();

        if (getSideOfAim == 1)
        {
            // target is on left side, move from right to left side
            bool pass = Side.TransferGun(rightSideGuns, leftSideGuns, rightSideGuns.guns.Count - 1);
            if (!pass)
            {
                Side.TransferGun(rightSideGuns, leftSideGuns, 0);
            }
        }
        if (getSideOfAim == -1)
        {
            // target is on left side, move from right to left side
            bool pass = Side.TransferGun(leftSideGuns, rightSideGuns, leftSideGuns.guns.Count - 1);
            if (!pass)
            {
                Side.TransferGun(leftSideGuns, rightSideGuns, 0);
            }
        }

    }

    /// <summary>
    /// finds direction of target relative to the ship
    /// </summary>
    /// <returns></returns>
    private int GetSideOfFocus()
    {
        var relativePoint = transform.InverseTransformPoint(target.position);
        if (relativePoint.x <= 0.0) // change into < later. and change 0.0s into something else to have better forward
            return -1;
        //print ("Object is to the left");
        else if (relativePoint.x > 0.0)
        {
            return 1;
            //print ("Object is to the right");
        } else
        {
            return 0;
            //print ("Object is directly ahead");
        }
    }

    private void ActivateGunBursts(bool active)
    {
        if (active)
        {
            int fireDirection = GetSideOfFocus();
            if (fireDirection == -1)
            {
                leftSideGuns.ActivateGuns(active);
            } else
            {
                rightSideGuns.ActivateGuns(active);
            }
        } else if (!active)
        {
            leftSideGuns.ActivateGuns(false);
            rightSideGuns.ActivateGuns(false);
        }
    }
    
    /// <summary>
    /// Chose path and go near first node
    /// </summary>
    /// <returns></returns>
    private IEnumerator FlyPath()
    {
        while (pathAroundTarget.Count > 0 && target)
        {
            
            bool isLastWp = pathAroundTarget.Count == 1;
            Vector3 point = pathAroundTarget.Dequeue();
            Debug.DrawLine(transform.position, point, Color.green, 5);
            yield return StartCoroutine(SceneSearching.MoveToPos(transform, unit, point, isLastWp, unit.speed*1.5f)); // higher speed means bigger allowed error

            //unit.ExecuteMove(SimpleUnit.Moves.forward, true);
            //yield return StartCoroutine(SceneSearching.Avoid(transform, target, null, null, 1));
        }
    }

    // move guns from 1 side to the other
    [System.Serializable]
    public class Side
    {
        public List<AutoFireGun> guns;

        public Side()
        {
            guns = new List<AutoFireGun>();
        }

        public static bool TransferGun(Side thisSide, Side newSide, int gunIndex)
        {
            AutoFireGun g = thisSide.guns[gunIndex];
            thisSide.guns.RemoveAt(gunIndex);
            newSide.guns.Add(g);
            ((MovableGun)g).ChangeSides();
            return true;
        }

        internal void ActivateGuns(bool active)
        {
            foreach (AutoFireGun gun in guns)
            {
                gun.AutoFireOn(active);
            }
        }
    }
    
}
