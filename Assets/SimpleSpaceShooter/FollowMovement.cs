using UnityEngine;
using System.Collections;

public class FollowMovement : SimplestMovement {

    Vector3 cross;
    float distance;
    Vector3 direction;

    public Transform target;
    public float minDist = 2;

    void Start() {
        movementVertical.SetLerpTo(1);
    }

	// Update is called once per frame
	new void Update () {
        //angle = Vector3.Angle(Vector3.zero - transform.position, Vector3.zero);
        var relativePoint = transform.InverseTransformPoint(target.position);
        direction = relativePoint;
        rotation.SetLerpTo(relativePoint.x);



        if (Vector3.Distance(target.position, transform.position) < minDist) {
            rotation.Reset();
            movementVertical.SetLerpTo(0);
        }else
	    {
            rotation.Reset();
            movementVertical.SetLerpTo(1);
        }

        base.Update();

    }
}
