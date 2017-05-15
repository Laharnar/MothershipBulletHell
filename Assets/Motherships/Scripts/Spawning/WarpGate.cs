using UnityEngine;
using System.Collections;

// warp gate layer: Warp
/// <summary>
/// Once empty formation unit enters the warp gate, warp it in by instantiating it
/// 
/// Leaders should be first unit that enters warp gate. If you really need to, you should shuffle the leader later
/// </summary>
public class WarpGate : MonoBehaviour {


    void OnTriggerEnter2D(Collider2D other) {
        // start warp in animation depending on unit size
    }

	void OnTriggerExit2D (Collider2D other) {
        FormationUnit emptyUnit = other.GetComponent<FormationUnit>();
        FormationTemplate emptyUnitPref = other.GetComponent<FormationTemplate>();
        if (emptyUnit) {
            Transform warpedUnit = WarpUnit(emptyUnitPref);
            if (warpedUnit) {
                FormationUnit warpFormationUnit = warpedUnit.GetComponent<FormationUnit>();
                warpFormationUnit.warpDone = true;
                // if warped unit is a leader, reconnect all its units to the actual unit model
                // for this reason, leaders shuold be first unit that enters warp gate
                bool isLeader = emptyUnit.setFollowers.Count > 0;
                if (isLeader) { // reassign all followers to new model
                    //emptyUnit.SetLeader(warpedUnit);
                    ReconnectAllFollowersToWarped(emptyUnit, warpFormationUnit);
                } else {
                    // not leader
                    // and has leader -> reassign warped unit to that leader

                    if (emptyUnit.leader && warpFormationUnit.assignToFormationLeaderOnWarpIn) {
                        warpFormationUnit.SetLeader(emptyUnit.leader);
                    } else {
                        // doesn't have leader -> let warped unit work on its own
                    }
                }
            }
            // get rid of warp-in-empty trash
            emptyUnit.CleanUp();
            Destroy(emptyUnit.gameObject);
        }
        
	}

    private void ReconnectAllFollowersToWarped(FormationUnit emptyUnit, FormationUnit warpedUnit) {
        for (int i = 0; i < emptyUnit.followingUnits.Count; i++) {
            emptyUnit.followingUnits[i].SetLeader(warpedUnit);
        }
        //warpedUnit.setFollowers = emptyUnit.followingUnits;
    }

    private Transform WarpUnit(FormationTemplate emptyUnit) {
        return Instantiate(emptyUnit.prefab, emptyUnit.transform.position, emptyUnit.transform.rotation) as Transform;
        
    }
}
