using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/*[
 
goal: create the mood of kamikazi - pointless rush towards death.

    It gets thown out of carrier by an automated mechanical hand.
The scans activate, and find their target.
It turns towards the target. It activates the engines until they are empty,
win or get lost in space.
When they explode, they sometimes go critical and take out nearby comrades.

    Note: they don't care who they hit, sometimes hitting friendlies.


 * */
/// <summary>
/// 14.5.2017
/// </summary>
public class SuicideBomber_v3 : AiBase {

    enum State {
        Startup,
        Scanning,
        Rush
    }

    public Vector2 movingInDir;// set this from mechanical hand once it lets go

    float startupTime = 0;

    public Targeting targeting;

    public SimpleUnit unit;
    public UnitInfo info;

	// Use this for initialization
	new void Awake () {
        base.Awake();
        targeting = new Targeting(initAlliance);
        startupTime = Time.time + 2;
        root = new DecisionMaxChoice("slow down after thrown out|scan|attack rush",
            new UtilityNode(Prepare,
                new ArgsFn(TimeSinceStarted, 200f)
            ),
            new UtilityNode(Scan,
                new ArgsFn(NoTarget, 100f)
            ),
            new UtilityNode(Aim,
                new ArgsFn(NotAiming, 20f)
            ),
            new UtilityNode(Suicide,
                new ArgsFn(HasTarget,50f)
            )
        );
	}

    void Prepare() {
        //todo: rotation to target doesn't work, it turns instantly.
        float timeTaken = 1f;
        Debug.Log(movingInDir.magnitude);
        Debug.Log(Vector2.one.magnitude / 3);
        if (movingInDir.magnitude > Vector2.one.magnitude/3) {
            movingInDir = Vector3.Slerp(movingInDir, Vector2.zero, Time.time - startupTime - timeTaken);
            transform.position = Vector3.MoveTowards(transform.position, (Vector2)transform.position+movingInDir, Time.deltaTime);
        } else {
            movingInDir = Vector2.zero;
            startupTime = Time.time + 2;
        }
    }

    void Scan() {
        // todo note: parent carrier should set target
        if (targeting.target == null && targeting.ScanArea(info, 3000)) {
            transform.up = (targeting.target.transform.position - transform.position);
        }
    }

    void Aim() {
        Debug.Log(Time.time + " "+startupTime);
        if (Time.time 
            < startupTime) {
            transform.up = ((targeting.target.transform.position - transform.position) * Time.deltaTime);
        }
    }

    void Suicide() {
        unit.ExecuteMove(SimpleUnit.Moves.forward);
    }

    //- maybe make all these function static and movre them to other file...
    // problems with passing this monobehaviour arg, add that option first.
    // scrap that, it's just more memory trash on script -

    float NotAiming(float f) {
        if (HasTarget(100) > 0) {
            Debug.Log(Vector2.Angle(targeting.target.transform.position - transform.position, transform.up));
            if (Vector2.Angle(targeting.target.transform.position - transform.position, transform.up) > 5) {
                return f+100;
            }
        }
        return 0;
    }

    float TimeSinceStarted(float f) {
        if (Time.time < startupTime) {
            return f;
        }
        return 0;
    }

    float NoTarget(float f) {
        if (targeting.target == null) { return f; } else {
            return 0;
        }
    }

    float HasTarget(float f) {
        return targeting.target != null ? f : 0;
    }

    // Update is called once per frame
    void Update () {
        root.Do();
	}
}

/// <summary>
/// Uses TargatableBase to find targets in scene.
/// </summary>
static class SearchManager {
    internal static UnitInfo ClosestEnemyUnit(UnitInfo info, string alliance) {
        UnitInfo[] t = new UnitInfo[TargetableBase.allTargets.Count];
        int i = 0;
        foreach (var item in TargetableBase.allTargets) {
            for (int j = 0; j < item.Value.Count; j++) {
                t[i] = item.Value[j];
                i++;
            }
        }
        return info.Closest(info.transform.position, alliance, t);
    }

    static UnitInfo Closest(this UnitInfo unit, Vector2 position, string alliance, UnitInfo[] objs) {
        float minDist = float.MaxValue;
        int r = -1;

        for (int i = 0; i < objs.Length; i++) {
            if (objs[i] == unit) continue;
            // filter same alliances
            if (objs[i].Get<TargetableBase>()) {
                if (objs[i].GetLast<TargetableBase>().initAlliance == alliance)
                continue;
            }

            // get closest
            float dist = Vector2.Distance(position, objs[i].transform.position);
            if (minDist > dist) {
                minDist = dist;
                r = i;
            }
        }
        if (r == -1) return null;
        return objs[r];
    }
}