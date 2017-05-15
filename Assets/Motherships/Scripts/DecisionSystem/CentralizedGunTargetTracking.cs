using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

/// <summary>
/// Keeps track of all targets that enter range of trigger collider.
/// 
/// This script should be on separate object/child, with collision detection layer, and then called from main script, so it allows custom layers, for collision optimization.
///
/// </summary>
[@RequireComponent(typeof(CircleCollider2D))]
public class CentralizedGunTargetTracking : MonoBehaviour {

    /// <summary>
    /// List of targets inside collider.
    /// </summary>
    List<TrackableTarget> targets = new List<TrackableTarget>();
    List<float> distances = new List<float>();
    List<List<GunBase>> attackedByGuns = new List<List<GunBase>>();// how many guns are aiming at the target

    //public Collider2D[] supportedCollisions;// which colliions will be considered for range

    public float distanceRefreshRate = 0.01f;// make different object have different referesh rate to avoid lag spikes

    // both delegates allow retargeting after out of range or destroyed or enter
    Action<Transform> onTargetExitRange;
    Action<Transform> onAquireTarget;
    Action onTargetDestroyed;

    public bool useDistanceRefresh = true;

    /// <summary>
    /// Which spaceship will receive message on target detection.
    /// </summary>

    //CircleCollider2D collider2d;

    #region Initialization
    void Start() {
        var collider2d = GetComponent<CircleCollider2D>();
        if (!collider2d.isTrigger) {
            Debug.Log("Setting collider to trigger.", this);
            collider2d.isTrigger = true;
        }

        StartCoroutine(RecalculateRanges());
    }

    public void RegisterTargetTracking(Action<Transform> onTargetExitRange, Action onTargetDestroyed, Action<Transform> onTargetEnterRange) {
        if (onTargetExitRange != null) {
            this.onTargetExitRange += onTargetExitRange;
        }
        if (onTargetDestroyed != null) {
            this.onTargetDestroyed += onTargetDestroyed;
        }
        if (onTargetEnterRange != null) {
            this.onAquireTarget += onTargetEnterRange;
        }
    } 
    #endregion

    #region Distance recalculating
    private IEnumerator RecalculateRanges() {
        // every x seconds recalculate all distances.
        bool stopCalcualtion = !useDistanceRefresh;
        while (true) {
            if (!stopCalcualtion && useDistanceRefresh) {
                RecaulculateRangesToAllTargets();
            }
            yield return new WaitForSeconds(distanceRefreshRate);
        }
    }

    private void RecaulculateRangesToAllTargets() {
        for (int i = 0; i < targets.Count; i++) {
            if (TargetNotDestroyed(i)) {
                RecalculateDistance(i);
            } else {
                i--;//since one was removed
            }
        }
    }

    private void RecalculateDistance(int i) {
        distances[i] = Vector3.Distance(transform.position, targets[i].transform.position);
    } 
    #endregion

    #region Target destroyed check
    private bool TargetNotDestroyed(int i) {
        if (targets[i] == null) {
            targets.RemoveAt(i);
            distances.RemoveAt(i);
            attackedByGuns.RemoveAt(i);

            OnTargetDestroyedMessage();
            //lost functionality: gun doesnt reset mode when target is lost
            return false;
        }
        return true;
    }

    /// <summary>
    /// Send message to all connected objects that some target was destroyed
    /// </summary>
    private void OnTargetDestroyedMessage() {
        if (onTargetDestroyed != null) {
            onTargetDestroyed();
        }
    } 
    #endregion

    #region Target enters range
    void OnTriggerEnter2D(Collider2D collider) {
        TrackableTarget possibleTarget = collider.gameObject.GetComponent<TrackableTarget>();
        if (possibleTarget == null || possibleTarget.shipSource == null) {
            return;
        }
        if (targets.Contains(possibleTarget)) {
            return;
        }
        // add new target
        targets.Add(possibleTarget);
        distances.Add(0);
        attackedByGuns.Add(new List<GunBase>());

        RecalculateDistance(targets.Count - 1);

        RebalanceTargets();

        OnAquiredTargetMessage(possibleTarget.transform);
    }


    private void OnAquiredTargetMessage(Transform target) {
        if (onAquireTarget != null) {
            onAquireTarget(target);
        }
    } 
    #endregion

    #region Target exits range
    void OnTriggerExit2D(Collider2D collider) {
        OnExitRange(collider);
    }

    /// <summary>
    /// Remove target and send message to all connected objects.
    /// </summary>
    /// <param name="collider"></param>
    private void OnExitRange(Collider2D collider) {
        TrackableTarget possibleTarget = collider.transform.GetComponent<TrackableTarget>();
        if (possibleTarget != null) {
            int i = targets.IndexOf(possibleTarget);
            if (i > -1) {
                targets.RemoveAt(i);
                distances.RemoveAt(i);
                attackedByGuns.RemoveAt(i);
            }

            OnTargetExitedRangeMessage(possibleTarget);
        }
    }

    /// <summary>
    /// Send message that some target exited range.
    /// </summary>
    /// <param name="possibleTarget"></param>
    private void OnTargetExitedRangeMessage(TrackableTarget possibleTarget) {
        if (onTargetExitRange != null) {
            if (possibleTarget.shipSource == null)
                Debug.Log("ship source is null. ", possibleTarget);
            else onTargetExitRange(possibleTarget.shipSource.transform);
        }
    }
    #endregion

    /// <summary>
    /// Reassigns guns to new targets to balance numbers of aims
    /// </summary>
    private void RebalanceTargets() {
        RecaulculateRangesToAllTargets();

        // free targets list vs reassign search
        List<int> freeTargets = GetNonprioritizedTargetsIDs();

        for (int i = 0; i < targets.Count; i++) {
            int numberOfGunsAttacking = attackedByGuns[i].Count;

            // retarget guns that are exceess
            if (numberOfGunsAttacking <= targets[i].targetingPriority) {
                continue;
            }

            int howManyToRetargetFotThisTarget = (numberOfGunsAttacking - 1) - targets[i].targetingPriority;// how many targets should be retargeted from this target
            
            // retarget all excess guns from this target either to targets that dont have enough, or random targets
            for (int j = howManyToRetargetFotThisTarget; j > -1; j--) {
                GunBase someGunOnThisTarget = attackedByGuns[i][j];

                if (freeTargets.Count > 0) {
                    Transform someFreeTarget = targets[freeTargets[0]].shipSource.transform;

                    someGunOnThisTarget.Retarget(someFreeTarget);
                    // if priority for this free target was achieved
                    if (attackedByGuns[freeTargets[0]].Count == targets[i].targetingPriority) {
                        freeTargets.RemoveAt(0);
                    }
                } else {
                    // all priority was used up, assign gun to random target
                    int r = UnityEngine.Random.Range(0, targets.Count);
                    someGunOnThisTarget.Retarget(targets[r].shipSource.transform);
                }
            }

        }
    }

    private List<int> GetNonprioritizedTargetsIDs() {
        List<int> freeTargets = new List<int>();
        for (int i = 0; i < targets.Count; i++) {
            if (attackedByGuns[i].Count < targets[i].targetingPriority) {
                freeTargets.Add(i);
            }
        } 
        return freeTargets;
    }

    /// <summary>
    /// returns spaceship that was closes to requested origin at last distance update
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Vector3 origin is deprecated"
    internal Transform AquireClosestAvaliableTarget(Vector3 origin, bool recalculateRangesFirst = true) {
        if (recalculateRangesFirst) {
            RecaulculateRangesToAllTargets();
        }
        if (distances.Count == 0) {
            return null;
        }
        float minDist = float.MaxValue;
        int mini = -1;
        for (int i = 0; i < distances.Count; i++) {
            if (AlreadyFittingPriority(i)) {
                continue;
            }

            if (distances[i] < minDist) {
                minDist = distances[i];
                mini = i;
            }
        }

        if (mini == -1) {
            mini = UnityEngine.Random.Range(0, targets.Count);
        }
        return targets[mini].shipSource.transform;
    }

    private bool AlreadyFittingPriority(int i) {
        // check if amount of guns aiming at this target is lower than demanded priority
        return targets[i].targetingPriority <= attackedByGuns[i].Count;
    }

    /// <summary>
    /// Assuming target's priority isnt fullfiled, register aim at target
    /// </summary>
    /// <param name="target"></param>
    internal void ConfirmAimAtTarget(GunBase origin, Transform target) {
        for (int i = 0; i < targets.Count; i++) {
            if (targets[i].shipSource.transform == target.transform) {
                attackedByGuns[i].Add(origin);
                break;
            }
        }
    }

    internal void CancelAimAtTarget(GunBase origin, Transform target) {
        for (int i = 0; i < targets.Count; i++) {
            if (targets[i].shipSource == null) {
                Debug.Log("Ship source isn't assigned", targets[i]);
            }
            if (targets[i].shipSource.transform == target.transform) {
                attackedByGuns[i].Remove(origin);
                break;
            }
        }
    }
}
