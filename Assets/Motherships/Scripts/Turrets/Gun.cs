using UnityEngine;  
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Gives player some control over guns.
/// 
/// Contains a few behaviours for guns.
/// TODO: separate user input and ai behaviour.
/// 
/// auto aim - aim at newest target
/// manual aim - aim at mouse and shoot turn on shooting with space, control locks rotation
/// directional aim - aim at point, activated by right click from any mode
/// target aim - aim at target if in range, else just in that direction, activated by right click on enemy from any mode
/// </summary>
public class Gun : AimingGunBase {

    // note: this script is enabled via DevGameState script that controls input

    public enum AimingState {
        autoAim, // aim by default behaviour, lifo i think
        manualAim, // aim at mouse
        directionAim, // aim at single vector
        targetAim // aim at specific target, until that target is down. If out of range, fire at secondary target.
    }

    /** targeting **/
    

    Transform secondaryTarget; // optional target object that targetting point follows if primary target is out of range, and aim state demands it
    bool followMouse = false;
    Vector2 lastMousePosition;

    AimingState aimState = AimingState.autoAim; // current state of aiming, auto is AI, other are player set states
    internal AimingState AimState { get { return aimState; } } // current state of aiming, auto is AI, other are player set states

    /** shooting **/
    public bool canFire; // disabling fire script diesn't work because it works with coroutine

    public float pauseTime = 0.7f; // how much time to begin firing again after losing target and re-aiming
    public bool spaceBarFireLockOff = true; // when true, gun will fire if its in manual mode

    // TODO: move this to simple gun
    public CentralizedGunTargetTracking tracking;

    protected override void OnStart() {
        // prepare gun's aim point
        CreateAimPoint();
        if (tracking) {
            tracking.RegisterTargetTracking(OnTargetOutOfRange, OnTargetDestroyed, OnTargetEnterRange);
        }

        base.OnStart();
    }


    protected override void FireLoopFrame(ref float time) {
        if (!canFire) {
            return;
        }

        // if in manual mode, use control to lock rotation, and spacebar to locks shooting, or rmb for shooting
        if (aimState == AimingState.manualAim) {
            HandlePlayerControls();
        }

        if (aimState == AimingState.directionAim) {
            // - untested -
            if (Input.GetKeyDown(KeyCode.Escape)) {
                aimState = AimingState.autoAim;
            }
        }

        // if gun is loaded and then fire
        if (fire || (aimState == AimingState.manualAim && spaceBarFireLockOff)) {
            base.FireLoopFrame(ref time);
        }

        // check if any of targets got destroyed
        TargetDestroyedCheck();

    }

    private void HandlePlayerControls() {
        // - untested -
        if (Input.GetKeyUp(KeyCode.L)) {
            spaceBarFireLockOff = !spaceBarFireLockOff;
        }
        if (Input.GetKeyUp(KeyCode.LeftControl)) {
            followMouse = false;
        }
        if (Input.GetMouseButtonUp(1)) {
            spaceBarFireLockOff = true;
        }
        if (Input.GetMouseButtonUp(1)) {
            spaceBarFireLockOff = false;
        }
    }

    #region Helper methods

    void OnTargetEnterRange(Transform target) {
        if (activeTarget == null) {
            AssignNewTarget(tracking.AquireClosestAvaliableTarget(transform.position).transform);
        }
    }

    /// <summary>
    /// additional check if target was destroyed before rescan was issued
    /// </summary>
    private void TargetDestroyedCheck() {
        // if its target got destroyed, reset aim
        if (activeTarget == null) {
            fire = false;
            //OnTargetDestroyed();//null);
            //break;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="newTarget">this target becomes secondary target in case of targetAim mode</param>
    /// <param name="targets"></param>
    private void AssignNewTarget(Transform newTarget, List<Transform> targets = null) {
        switch (aimState) {
            case AimingState.autoAim:
                fire = true;
                followMouse = false;

                secondaryTarget = null;

                if (newTarget) {
                    //for retargeting
                    if (activeTarget) {
                        tracking.CancelAimAtTarget(this, activeTarget);
                    }

                    activeTarget = newTarget;// by rule aim at newest enemy. At the moment(4.9.2016) that's closest one
                    if (tracking) {
                        tracking.ConfirmAimAtTarget(this, newTarget);
                    }
                    StartCoroutine(ResumeFire(pauseTime));
                }/*else
                // if there is no new target, just aim at one of old
                if (activeTarget == null && targets.Count > 0) {
                    activeTarget = targets[targets.Count - 1];
                }*/
                else fire = false;
                break;
            case AimingState.manualAim:
                // here acualy mouse takes over, so it doesn't matter what target it has
                //activeTarget = newTarget.transform;// by rule aim at newest enemy
                followMouse = true;
                activeTarget = null;
                secondaryTarget = null;
                break;
            case AimingState.directionAim:
                // he targets also doesn't matter, since its direction aim
                followMouse = false;
                activeTarget = null;
                secondaryTarget = null;
                break;
            case AimingState.targetAim:
                // Here rule applies to secondary target, which is attacked if primary is out of range
                // Don't touch primary target here.
                followMouse = false;
                if (newTarget == activeTarget) {//if primary target reentered range, resume aiming at it
                    AimAtTarget(activeTarget);
                    StartCoroutine(ResumeFire(pauseTime));
                } else
                    secondaryTarget = newTarget.transform; 
                break;
            default:
                break;
        }
    }

    

   
    #endregion Helper methods
    /// <summary>
    /// Function called via gun tracking delegate
    /// 
    /// Maybe Todo: move it to custom gun tracking behaviour.
    /// 
    /// Date added: 4.9.2016
    /// </summary>
    /// <param name="target"></param>
    void OnTargetOutOfRange(Transform target){
        if (this.activeTarget == target) {
            tracking.CancelAimAtTarget(this, target);
            Transform tar = tracking.AquireClosestAvaliableTarget(transform.position);
            if (tar)
	        {
                switch (aimState) {
                    case AimingState.autoAim:
                        activeTarget = null;
                        AssignNewTarget(tar/*,targets*/);
                        break;
                    case AimingState.manualAim: // this state can't happen
                        break;
                    case AimingState.directionAim: // this state can't happen
                        break;
                    case AimingState.targetAim:
                        // keep shooting after where primary was target even if out of range
                        ClearAim();
                        /*if (secondaryTarget) {
                            AimAtTarget(secondaryTarget);
                            StartCoroutine(ResumeFire(pauseTime));
                        }*/
                        break;
                    default:
                        break;
                }
            }
        }
    }

    internal override void ClearAim() {
        base.ClearAim();
        followMouse = false;
    }

    /// <summary>
    /// Fired when on target is destroyed is detected
    /// 
    /// Date added: 4.9.2016
    /// </summary>
    void OnTargetDestroyed() {
        if (activeTarget == null) {
            Transform tar = tracking.AquireClosestAvaliableTarget(transform.position);
            if (tar) {
                switch (aimState) {
                    case AimingState.autoAim:
                        //if (targets.Count != 0) {
                        AssignNewTarget(tar, /*targets*/ null);
                        //}
                        break;
                    case AimingState.manualAim:// this state can't happen, since player controls it
                        break;
                    case AimingState.directionAim:// this state can't happen, since target is point, not object
                        break;
                    case AimingState.targetAim:
                        // if primary target was destroyed, aim at secondary, and exit this state
                        if (secondaryTarget != null) {
                            aimState = AimingState.autoAim;
                            AssignNewTarget(secondaryTarget, null);
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void OnExitRange(Transform other) {
        if (other == activeTarget) {
            switch (aimState) {
                case AimingState.autoAim:
                    activeTarget = null;
                    AssignNewTarget(other, /*targets*/ null);
                    break;
                case AimingState.manualAim: // this state can't happen
                    break;
                case AimingState.directionAim: // this state can't happen
                    break;
                case AimingState.targetAim:
                    // keep shooting after where primary was target even if out of range
                    ClearAim();
                    /*if (secondaryTarget) {
                        AimAtTarget(secondaryTarget);
                        StartCoroutine(ResumeFire(pauseTime));
                    }*/
                    break;
                default:
                    break;
            }
        }
    }




    #region Change aim mode
    internal void FocusFire(Transform primaryTarget) {
        // is this function broken?
        
        aimState = AimingState.targetAim;
        activeTarget = primaryTarget; // chose new target doesnt handle primary, but secondary target, for this mode
        AssignNewTarget(primaryTarget);
    }


    internal void FocusFire(Vector3 world) {
        aimState = AimingState.directionAim;
        ClearAim(world);
        AssignNewTarget(null);
    }

    /// <summary>
    /// Clears all targets and begins to follow mouse
    /// </summary>
    internal void StartManualAim() {
        aimState = AimingState.manualAim;
        AssignNewTarget(null);
    }

    internal void StartAutoAim() {
        aimState = AimingState.autoAim;
        AssignNewTarget(null);
    } 
    #endregion

    protected override Quaternion CalculateRotation(Vector3 aimDir) {
        if (aimState == AimingState.manualAim) {
            if (followMouse) {
                lastMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            aimDir = lastMousePosition;
        }

        return base.CalculateRotation(aimDir);
    }

    internal override void TargetDownCallback(Transform destroyedTarget) {
        OnTargetDestroyed();
    }

    internal override void Retarget(Transform target) {
        AssignNewTarget(target);
    }
}
