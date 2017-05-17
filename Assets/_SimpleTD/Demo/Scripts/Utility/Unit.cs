using UnityEngine;
using System.Collections;

[@RequireComponent(typeof(CircleCollider2D))]
[@RequireComponent(typeof(Rigidbody2D))]
[@RequireComponent(typeof(Engine))]
public class Unit : MonoBehaviour {

    public CircleCollider2D collider;
    public Rigidbody2D rig;

    WayPath travel;
    bool stop = false;

    public UnitWorth unitStats = new UnitWorth();
    public int livesWorth { get { return unitStats.livesWorth; } set { unitStats.livesWorth = value; } }

    public void Init(Waypoint startingWaypoint) {
        print(startingWaypoint);
        transform.position = startingWaypoint.transform.position;

        travel = new WayPath(startingWaypoint, GetComponent<Engine>());

        Init();
    }

    void Start() {
        StartCoroutine(TravelControl());
    }


    public void Init() {
        if (collider == null) {
            collider = GetComponent<CircleCollider2D>();
        }
        if (rig == null) {
            rig = GetComponent<Rigidbody2D>();
        }

        if (!rig.isKinematic) {
            rig.isKinematic = true;
        }

        if (collider.isTrigger == false) {
            collider.isTrigger = true;
        }
    }

    /// <summary>
    /// Go to next wp when arriving at wp
    /// </summary>
    /// <param name="depth"></param>
    /// <returns></returns>
    IEnumerator TravelControl(int depth = -1) {
        int curDepth = 0;
        while (curDepth < depth || depth == -1) {
            if (travel.move.arrived) {
                curDepth++;
                travel.NextWay();
            }
            yield return null;
        }
    }


}

[System.Serializable]
public class UnitWorth {
    public int livesWorth;
}

class WayPath {
    Waypoint lastWay;
    Waypoint targetWay;

    public Engine move;

    //private Waypoint startingWaypoint;

    public WayPath(Waypoint startingWaypoint, Engine movement) {
        this.targetWay = startingWaypoint;
        this.move = movement;
        NextWay();
    }

    internal void NextWay() {
        lastWay = targetWay;
        if (targetWay == null) {
            Stop();
            return;
        }
        targetWay = targetWay.ChoseOne();
        if (targetWay == null) {
            Stop();
            return;
        }
        // TODO: Obsolete, replace it with global implementation
        move.MoveTowards(targetWay.transform.position);
    }

    private void Stop() {
        move.stop = true;
        if (lastWay.GetType() == typeof(EndingWaypoint)) {
            ((EndingWaypoint)lastWay).ArrivedAtEnd(move);
        }
    }
}