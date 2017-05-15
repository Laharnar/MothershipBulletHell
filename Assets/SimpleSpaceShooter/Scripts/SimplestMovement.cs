using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Use SetLerpTo on rotation and movement that you need
/// </summary>
public class SimplestMovement : MonoBehaviour {

    public Transform movementVerticalTarget;
    public Transform movementHorizontalTarget;
    public Transform rotateTarget;

    // use this to restrict movement in bounds, like camera, or boss arena
    public bool clampPositionInBounds = false;
    protected Rect boundsRect; // this value has to be manualy set from other script

    public LerpVal movementVertical = new LerpVal() { axis = Vector3.up };
    public LerpVal movementHorizontal = new LerpVal() { axis = Vector3.right };
    public LerpVal rotation = new LerpVal() { axis = Vector3.back };

    public bool moveOnEnabled = true;

    public bool activateWithUtility = false;
    public ActivateBehaviourUtility utility;

    protected void Awake() {
        if (!movementHorizontalTarget) movementHorizontalTarget = transform;
        if (!movementVerticalTarget) movementVerticalTarget = transform;
        if (!rotateTarget) rotateTarget = transform;

        if (movementVertical.axis == Vector3.zero)
            movementVertical.axis = Vector3.up;
        if (movementHorizontal.axis == Vector3.zero)
            movementHorizontal.axis = Vector3.right;
        if (rotation.axis == Vector3.zero)
            rotation.axis = Vector3.back;

        if (movementVertical.use && movementVertical.useOnStart) movementVertical.SetLerpTo(movementVertical.axis.y);
        if (movementHorizontal.use && movementHorizontal.useOnStart) movementHorizontal.SetLerpTo(movementHorizontal.axis.x);
        if (rotation.use && rotation.useOnStart) rotation.SetLerpTo(rotation.axis.z);

        if (activateWithUtility) {
            utility.Register(this);
        }
    }

    internal void OnCustomEnabled() {
        if (moveOnEnabled)
            SetMoveLerp(0, 1);
    }

    protected void Update() {
        movementVerticalTarget.Translate(movementVertical.GetUpdate());
        movementHorizontalTarget.Translate(movementHorizontal.GetUpdate());
        rotateTarget.Rotate(rotation.GetUpdate());

        if (clampPositionInBounds) {
            transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, boundsRect.xMin, boundsRect.xMax),
            Mathf.Clamp(transform.position.y, boundsRect.yMin, boundsRect.yMax),
            transform.position.z);
        }
    }

    internal void SetMoveLerp(int h, int v, bool bounceEffectHorizontal = false, bool bounceEffectVertical = false, bool resetRotation = false) {
        if (!bounceEffectHorizontal) movementHorizontal.Reset();
        if (!bounceEffectVertical) movementVertical.Reset();
        movementVertical.SetLerpTo(v);
        movementHorizontal.SetLerpTo(h);
    }

    [System.Serializable]
    public class LerpVal {
        public bool use = true;
        public bool useOnStart = false;
        public bool useAcceleration = true;

        public Vector3 axis = Vector3.forward;

        public float maxSpeed = 20; // speed
        public float accelerationStep = 1; // step per frame
        public float deccelerationStep = 1;

        float lerpTo;
        float lastTo;

        float start;
        float time;
        float cur;

        /// <summary>
        /// Updates when there is a change
        /// </summary>
        /// <param name="value"></param>
        public void SetLerpTo(float value, bool updateAnyway = false) {
            value = Mathf.Clamp(value, -1, 1);
            if (!useAcceleration) {
                if (value < 0) value = -1;
                else if (value > 0) value = 1;
            }
            if (value != lastTo) {
                lastTo = lerpTo;
                lerpTo = Mathf.Clamp(value, -maxSpeed, maxSpeed);

                start = cur;
                time = 0;
            }
        }

        public Vector3 GetUpdate() {
            if (!use) return Vector3.zero;

            //lerp values
            cur = Mathf.Lerp(start, lerpTo, time);// htime/speed because parameter is 0-1, and time goes up to steering. time+steering_/2 to get rid of negative part

            if (lerpTo == 0)
                time += Time.deltaTime * deccelerationStep;
            else time += Time.deltaTime * accelerationStep;
            float move = cur * maxSpeed;
            return axis * move * Time.deltaTime;
        }

        internal void Reset() {
            cur = 0;
        }
    }
}
