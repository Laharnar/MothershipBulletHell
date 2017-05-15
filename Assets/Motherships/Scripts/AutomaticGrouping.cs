using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// Put on all units that can use formations.
/// 
/// Every unit gets a chance to be put as a leader, different types can have different chances.
/// Leader changes when leader is destroyed or unit loads.
/// 
/// Formations apply until someone in formation gets destroyed or formation leader enters target range. 
/// Then it's free for all, units cant be lead if the are close to target.
/// </summary>
public class AutomaticGrouping : MonoBehaviour {

    public Bomber_v2 bomber_v2;// replace this with info.sceneUser.target(add target to it)
    //public ShipInfo info;
    
    public float chanceForThisUnit = 0.2f;
    public float timeOutOfRange { get; private set; }

    internal bool isLeader = false;

    bool canBeLead = true;
    public float everyManForHimselfRange;
    internal FormationData formation;// formation manager assigns formation to this group when it joins

    private float generatedLeaderValue;


    // Use this for initialization
    void Awake () {
        SetIsLeader();
	}

    private void SetIsLeader() {
        generatedLeaderValue = UnityEngine.Random.Range(0.0f, 1.0f);
        isLeader = generatedLeaderValue <= chanceForThisUnit;
        FormationManager.scene.AddUnit(this, isLeader);
    }

    // Update is called once per frame
    void Update () {
        if (Vector3.Distance(bomber_v2.target.position, transform.position) < everyManForHimselfRange) {
            if (isLeader) {
                FormationManager.scene.RemoveUnit(this, isLeader);
                isLeader = false;
                formation = null;
            }
            AbandonFormation();
            bomber_v2.travelTo.applySecondaryTravel = false;
            timeOutOfRange = 0;
        } else {

            timeOutOfRange += Time.deltaTime;
            if ((int)timeOutOfRange % 2 == 0)
            if (isLeader) {
                if (formation == null)
                    FormationManager.scene.AddUnit(this, isLeader);
                bomber_v2.travelTo.applySecondaryTravel = false;
                formation.UpdateSize(timeOutOfRange);

                // put units in formation
                formation.ApplyFormation(this, formation.assignedChildren);//put in timed enumerator upddate --performnce
            } else {
                bomber_v2.travelTo.applySecondaryTravel = true;
            }
        }


    }

    private void AbandonFormation() {
        FormationManager.scene.RemoveUnit(this, isLeader);
    }
}

