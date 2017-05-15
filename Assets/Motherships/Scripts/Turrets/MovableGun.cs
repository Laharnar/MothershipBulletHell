using UnityEngine;
using System.Collections;

public class MovableGun : AutoFireGun {

    public Transform point1;
    public Transform point2;

    public bool only1Point = false;
    bool activePoint = true;// true:point1, false:point2

    private void Awake() {
        if (only1Point) {
            if (point1 == null && point2 == null) Debug.LogError("No points assigned", this);
        } else if (point1 == null || point2 == null) Debug.LogError("One of points isn't assigned", this);
    }


    public void ChangeSides() {
        activePoint = !activePoint;

        if (activePoint) MoveTo(point1);
        else MoveTo(point2);
    }

    void MoveTo (Transform target) {
        transform.position = target.position;
        transform.rotation = target.rotation;
    }
}
