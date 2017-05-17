using UnityEngine;
using System.Collections;

/// <summary>
/// Directs aim empty towards target, so that spawned projectiles can be aimed in same direction
/// </summary>
public class AimControl : MonoBehaviour {
    internal Transform target;

    public Transform rotateObject;

    void Update()
    {
        UpdateAim();
    }

    private void UpdateAim()
    {
        if (target)
        {
            rotateObject.right = target.position - transform.position;
            Debug.DrawLine(transform.position, target.position);
        }
    }

    public void SetTarget(Transform ntarget)
    {
        this.target = ntarget;
        UpdateAim();
    }
}
