using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SimpleGroupUnit : MonoBehaviour {

    public float speed = 2;

    public Group group;
    internal string unitType;

    internal SceneDependantAI source;
    private List<SimpleGroupUnit> followers = new List<SimpleGroupUnit>();
    public LineFormation formation;

    public float allowedError = 0.2f;

    // has joined formation for first time. reset every time formation changes
    bool joinedFormation = false;
    bool behindToAheadChange = false; // this has to trigger once for joined formation to work
    bool behind, ahead;

    // Use this for initialization
    void Start () {
        source = GetComponent<SceneDependantAI>();
        
        source.getSource.onMoveBy += OnMoveLeader;
        if (group)
        {
            group.RegisterUnit(this);
        }
    }

    /// <summary>
    /// group version of on move that tries to use formation
    /// moves towards designated point in formation
    /// Note: small hiccups in movement happen because move mode changes from formation modified slowdown
    ///  to follow point, which is still in idle state
    /// </summary>
    /// <param name="moveDone"></param>
    /// <param name="rotationDone"></param>
    /// <param name="leader"></param>
    void OnMoveLeader(Vector3 leadMoveDone, Vector3 leadRotationDone)
    {
        if (group.leader != this)  return;
        foreach (SimpleGroupUnit follower in followers)
        {
            // skip self
            if (follower.transform == transform) continue;
            
            //move done by leader is somehow modified. so we need actual move
            // comment this line to ignore formations and just copy movement
            bool lastBehind = behind;
            bool lastAhead = ahead;

            // modify movement for this unit, note: leadMoveDone/rota is changed by this function
            Vector3 followerTargetMp = formation.Follower(
            ref leadMoveDone, ref leadRotationDone,
            follower, group.leader, allowedError, out ahead, out behind);

            Vector3 modifiedLeadMove = leadMoveDone;
            Vector3 modifiedLeadRota = leadRotationDone;
            
            // first time for joining formation, behvaiour is different for it
            if (lastBehind != behind || lastAhead != ahead)
            {
                joinedFormation = true;
            }

            float speedModifier = 1;
            float nearRange = 0;

            float followRange = 2;
            float stayCloseRange = 0.1f;

            if (behind)
            {
                // first time quicly accelerate to its post, then slowly hover up and down
                speedModifier = joinedFormation ? formation.followModifier : formation.slowCatchUpModifier;
                nearRange = joinedFormation ? followRange : stayCloseRange;
                SceneSearching.MoveTo(transform, follower.source.getSource, followerTargetMp, nearbyRange: nearRange, nearbyToTargetSpeedMod: speedModifier);

                //follower.source.MoveTo(followerTargetMp, nearbyRange: nearRange, nearbyToTargetSpeedMod: speedModifier);
            }

            if (ahead)
            {
                Vector3 moveMod = Vector3.one;
                follower.source.Idle();
                if (joinedFormation)
                {
                    moveMod = modifiedLeadMove;
                }
                else
                {
                    Vector3 fullMove = group.leader.source.getSource.fullPossibleMove;
                    Vector3 fullRota = group.leader.source.getSource.fullPossibleRotation;
                    moveMod = fullMove * formation.slowModifier;
                }
                follower.transform.Translate(
                        moveMod * (1 - follower.source.getSource.getVertical));// make sure new moveemnt inst boosted over last decaying one
                follower.transform.Rotate(
                    modifiedLeadRota * (1 - follower.source.getSource.getHorizontal));// make sure new moveemnt inst boosted over last decaying one
            }
        }
        
        /*
        // old version where only rotation and movement is copied from leader
        for (int i = 0; i < followers.Count; i++)
        {
            if (followers[i].transform == transform) continue;
            //move done by leader is somehow modified. so we need actual move

            followers[i].transform.Translate(moveDone);
            followers[i].transform.Rotate(rotationDone);
        }*/
    }

    /// <summary>
    /// call this on leader that has his group copy his movement
    /// </summary>
    /// <param name="toPosition"></param>
    /// <param name="units"></param>
    /// <param name="formation"></param>
    internal void GroupMoveTo(Vector3 toPosition, List<SimpleGroupUnit> units, LineFormation formation)
    {
        /*this.formation = formation;
        this.followers = units;
        */
        float spd = formation.LeaderSpeed();
        SceneSearching.MoveTo(transform, null, toPosition, farSpeedMod : 1);
        
        //source.MoveTo(toPosition, farSpeedMod: formation.LeaderSpeed());
    }
}
