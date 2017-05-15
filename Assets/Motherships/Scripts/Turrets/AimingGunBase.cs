using UnityEngine;
using System.Collections;

public class AimingGunBase : GunBase {

    public float turnSpeed = 8;

    Transform _activeTarget;
    public Transform activeTarget {
        protected set {
            _activeTarget = value; 
            AimAtTarget(_activeTarget);
        }
        get { return _activeTarget; }
    } // actual target object that targetting point follows(is parented to)

    // empty game object where gun tries to points at. follows active target, or moves in specific behaviour
    Transform _aimDirection;
    protected Transform aimDirection {
        get {
            if (_aimDirection == null) 
                CreateAimPoint(); 
            return _aimDirection;
        }
        set { _aimDirection = value; }
    }

    float timer = 0;
    float curve;

    // Update is called once per frame
    protected void Update() {
        // always rotate toward aim point
        Rotation();
    }

   
    /// <summary>
    /// Prepare gun's aim point by recreating the object
    /// </summary>
    protected void CreateAimPoint() {
        aimDirection = new GameObject().transform;
        //aimDirection.parent = transform; //parent it to gun to create constant motion in that direction.
        aimDirection.position = spawnPoint.position;
        aimDirection.name = "AIM_" + transform.name;
    }

    protected void AimAtTarget(Transform target) {
        timer = 0;
        // this also handles null values just fine
        aimDirection.parent = target;
        if (target != null) {
            aimDirection.localPosition = Vector2.zero;
        }
    }

    internal virtual void ClearAim() {
        aimDirection.parent = null;
    }

    internal void ClearAim(Vector3 aimAtWorld) {
        ClearAim();
        aimDirection.position = aimAtWorld;
    }

            public AnimationCurve accelerationCurve;
    float lerp = 0;
    protected virtual void Rotation() {
        // rotate towards aim object
        Color col = Color.clear;
        if (name.Contains("StandardCannon")) {
            col = Color.grey;
        }
        if (name.Contains("LRLauncher")) {
            col = Color.yellow;
        }
        if (activeTarget) {

            Debug.DrawLine(transform.position, activeTarget.position, col, 0.1f);
        }


        Vector3 aimDir = aimDirection.position;
        var newRotation = CalculateRotation(aimDir);
        float norm = timer/10;// *(transform.rotation.z / newRotation.z);
        curve = accelerationCurve.Evaluate(norm);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, /*accelerationCurve.Evaluate(norm)*/accelerationCurve.Evaluate(Time.deltaTime * turnSpeed));
        timer += Time.deltaTime;
    }

    protected virtual Quaternion CalculateRotation(Vector3 aimDir) {

        // try to look at target
        aimDir.z = 0;
        Quaternion newRotation = Quaternion.LookRotation(transform.position - aimDir, Vector3.forward);
        newRotation.x = 0.0f;
        newRotation.y = 0.0f;
        return newRotation;
    }


}
