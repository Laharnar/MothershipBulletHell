using UnityEngine;
using System.Collections.Generic;

public class GroupGunControl : MonoBehaviour {

    public ShipInfo source;

    //UnitsInScene scene;
    public SimpleGun[] guns;
    /// <summary>
    /// sorted by distances
    /// </summary>
    TargetsDistanceSortedList<Component> targetsSorted = new TargetsDistanceSortedList<Component>();

    float t;
    public float retargetRate = 2f;// how fast are distances to targets recomuputed, allow player to modify it.
    private List<Transform> targetUnits;
    private List<float> targetDistances;

    // Use this for initialization
    void Start () {
        if (!source)
            source = GetComponent<ShipInfo>();
        guns = GetComponentsInChildren<SimpleGun>();

        if (guns.Length == 0) {
            Debug.Log("Guns aren't assigned.", this);
        }
        // less/more guns than targets
        UpdateAimOnGuns_v2();
    }

    /// <summary>
    /// Guns get closes targets to the unit. each gun gets its own target from all targets.
    /// Targets can have multiple guns attacking them.
    /// Guns dont can change target if they have it at some % chance per gun
    /// </summary>
    void UpdateAimOnGuns_v2() {
        // if there is no targets, guns will reset fire.
        if (targetsSorted.Count == 0) {
            foreach (var gun in guns) {
                gun.shootingLogic.HoldFire();
            }
            return;
        }

        int lastGun = -1;
        int offset = 0;// how many nulls were skipped
        for (int i = 0; i < guns.Length; i++) {
            /*if (guns[i].hasTarget) { // this allows enemy units go get closer than they should cause there isnt enought spread fire
                continue;
            }*/

            // get index 1. skip null target 2. target targets in reverse order, up 3. if there are more guns than targets, assign multiple guns to same target
            // skip null targets, but dont skip guns
            int targetIndex = i+offset;
            while (targetIndex < targetsSorted.Count && targetsSorted[targetIndex].target == null) {
                offset++;
                targetIndex++;
            }
            targetIndex = (targetsSorted.Count - 1) - (i + offset) % targetsSorted.Count;
            guns[i].shootingLogic.SetTarget((Transform)targetsSorted[targetIndex].target);
            lastGun = i;
        }
    }

    private void UpdateAimOnGuns() {
        if (targetsSorted.Count == 0) return;
        int lastGun=0;
        for (int i = 0; i < guns.Length; i++) {
            if (targetsSorted[i % targetsSorted.Count].target== null) {
                continue;
            }
            guns[i].shootingLogic.SetTarget((Transform)targetsSorted[i % targetsSorted.Count].target);
            lastGun = i;
        }
        for (int i = lastGun+1; i < guns.Length; i++) {
            guns[i].shootingLogic.HoldFire();
        }
    }



    // Update is called once per frame
    void Update () { 
        if (Time.time > t) {
            t = Time.time + retargetRate;
            SceneSearching.GetDistanceSorted(transform.position, UnitsInScene.GetEnemyShips(source.flag.alliance), ref targetsSorted);
        }
        

        UpdateAimOnGuns_v2 ();
	}
}
