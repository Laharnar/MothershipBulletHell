using UnityEngine;
using System.Collections;

[System.Obsolete("script was integrated directly into fighter")]
public class GunControl : MonoBehaviour {
    public SimpleGun forwardGun;
    public SimpleGun rotatingGun;
    
    internal void FireRotatingGuns(Transform activeTarget) {
        rotatingGun.shootingLogic.SetTarget(activeTarget);
        //rotatingGun.FocusFire(activeTarget);
        //rotatingGun.fire = true; // gun should start firing automaticaly
    }

    internal void FireForwardGuns() {
        forwardGun.shootingLogic.Fire();
    }

    internal void CancelFireForwardGuns() {
        forwardGun.shootingLogic.HoldFire();
    }

    /*
    public AutoFireGun forwardGun;

    public Gun rotatingGun;

    internal void FireRotatingGuns(Transform activeTarget) {
        rotatingGun.FocusFire(activeTarget);
        //rotatingGun.fire = true; // gun should start firing automaticaly
    }

    internal void FireForwardGuns() {
        forwardGun.AutoFireOn(true);
    }

    internal void CancelFireForwardGuns() {
        forwardGun.AutoFireOn(false);
    }

    internal void CancelFireRotatingGuns(Transform activeTarget) {
        //rotatingGun.FocusFire(activeTarget);
        //rotatingGun.fire = false;// gun should stop firing automaticaly
    }*/
}
