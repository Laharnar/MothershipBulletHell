using UnityEngine;
using System.Collections;

public class LaserGun : Gun {

    protected override void OnStart() {
        CreateAimPoint();
        base.OnStart();

    }


    internal void Init(Structure prefabConnection) {
        transform.parent = prefabConnection.parentOnBuilt;
        transform.localPosition = Vector3.zero;
        transform.localRotation = new Quaternion();
    }

    protected override void FireLoopFrame(ref float time) {

        if ((!activeTarget)&& Reaim()) {
            fire = true;
        } 
        base.FireLoopFrame(ref time);
    }

    private bool Reaim() {
        Transform targetChosen = UnitsInScene.GetClosestShip(transform, 2);
        if (targetChosen) {
            AimAtTarget(targetChosen);// *2 to get less calculations
            activeTarget = targetChosen;
        }
        return activeTarget != null;
    }

    internal override void TargetDownCallback(Transform destroyedTarget) {
        Reaim();
    }
}
