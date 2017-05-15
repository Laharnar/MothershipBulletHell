using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// allows Syncronization of movement with some other unit
/// </summary>
public class FormationUnit : MonoBehaviour {

    public FormationUnit leader;

    internal List<FormationUnit> followingUnits { get; private set; }
    internal List<FormationUnit> setFollowers { get {
        if (followingUnits == null) {
            followingUnits = new List<FormationUnit>();
        }
        return followingUnits; } set { followingUnits = value; } }

    public EngineControl engines;

    public float structuralSizeOfShip;

    public bool followRotation = true;
    public bool followPosition = true;
    private bool posSync;
    private bool rotSync;

    public bool assignToFormationLeaderOnWarpIn;

    public bool warpDone { get; set; }

    void Start() {
        

        if (leader) {
            SetLeader(leader);
        }
        if (!leader && (followPosition || followRotation)) {
            Logger.AddLog("Default leader isn't assigned.", this);
        }
    }

    public void SetLeader(FormationUnit newLeader) {
        // if leadre is alredy there, unregister from it
        if (leader) {
            //CancelRegistration(leader, this);
        }
        // if we have non null new leader, stop all engines, and copy movement from leader
        if (newLeader) {
            engines.SuspendAll();
            FollowSomeUnit(newLeader);
        } else {
            // no leader means engines have to resume
            engines.ResumeAll();
        }
    }

    public void CancelRegistration(FormationUnit leader, FormationUnit source) {
        if(leader.engines)
            leader.engines.CancelRegistration(source.transform);
        if (leader.setFollowers.Count > 0) {
            leader.setFollowers.Remove(source);
        }
    }

    /// <summary>
    /// This is on non leader
    /// </summary>
    /// <param name="someOtherUnit"></param>
    private void FollowSomeUnit(FormationUnit someOtherUnit) {
        someOtherUnit.RegisterFollower(this);
    }

    /// <summary>
    /// This is on leader
    /// </summary>
    /// <param name="newFollower"></param>
    private void RegisterFollower(FormationUnit newFollower) {
        // register this unit to follow some other unit.
        newFollower.leader = this;
        if (followingUnits == null) {
            followingUnits = new List<FormationUnit>();
        }
        if (newFollower != this) {// can't self leader
            if (followingUnits.Contains(newFollower)) {
                Debug.Log("double entry");
            } else {
                followingUnits.Add(newFollower);
                engines.RegisterFollower(newFollower.transform, newFollower.OnPosChange, newFollower.OnRotChange);
            }
        }

        
    }

    void OnPosChange(EngineControlPanel posSource, Vector3 oldPos, Vector3 newPos) {
        posSync = false;

        // some source is still connected to this unit, even though this unit doesn't want that
        FormationUnit source =posSource.GetComponent<FormationUnit>();
        if (source != leader) {
            CancelRegistration(source, this);
        }

        // sync position, no limits
        if (followPosition && leader) {
            posSync = true;
            //print("");
            transform.position = transform.position + (newPos - oldPos);
        }
    }

    void OnRotChange(EngineControlPanel rotSource, Vector3 oldRot, Vector3 newRot) {
        rotSync = false;
        // sync rotation, no limits
        if (followRotation && leader && rotSource.transform == leader.transform) {
            rotSync = true;
            transform.eulerAngles = newRot;
        }
    }

    internal void CleanUp() {
        if (leader) {
            CancelRegistration(leader, this);            
        }
    }

}