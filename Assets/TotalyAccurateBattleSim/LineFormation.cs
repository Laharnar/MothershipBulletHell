using UnityEngine;
using System.Collections;
using System;

public class LineFormation : MonoBehaviour {

    // leader moves with normal to fast speed, the rest try to catch up to him by modifying speed
    public float slowWaitModifier = 0.35f;// slow wait for then
    public float slowModifier = 0.70f; // little behind
    public float normalModifier = 0.75f; // leader's speed
    public float followModifier = 0.80f; // little ahead, slow down
    public float slowCatchUpModifier = 0.90f;// slow catch up
    public float fastModifier = 1f; // full speed

    /// <summary>
    /// Linear follow leader
    /// </summary>
    /// <param name="moveDone"></param>
    /// <param name="rotationDone"></param>
    /// <param name="unit"></param>
    /// <param name=""></param>
    internal Vector3 Follower(ref Vector3 moveDone, ref Vector3 rotationDone
        , SimpleGroupUnit unit, SimpleGroupUnit leader, float allowedError, out bool ahead, out bool behind)
    {
        Vector3 fullMove = leader.source.getSource.fullPossibleMove;
        Vector3 fullRota = leader.source.getSource.fullPossibleRotation;

        Vector3 targetPoint = leader.transform.position + Vector2.Dot(unit.transform.position - leader.transform.position
            , leader.transform.right)
            / Vector2.Dot(leader.transform.right, leader.transform.right) 
            * leader.transform.right;

        Vector3 dir = targetPoint - unit.transform.position;

        behind = dir.y > allowedError; // or > 0
        ahead = dir.y < -allowedError;

        // draw line to linear point on leader's right side
        Debug.DrawLine(unit.transform.position, targetPoint + new Vector3(1, 0, 0), Color.blue);
        Debug.DrawLine(unit.transform.position, ahead ? unit.transform.position + unit.transform.up
           : behind ? unit.transform.position - unit.transform.up : unit.transform.position, Color.red);

        moveDone = ahead ? fullMove * slowWaitModifier : behind ? fullMove * fastModifier : fullMove * normalModifier;

        return targetPoint;
    }

    internal float LeaderSpeed()
    {
        return normalModifier;
    }
}
