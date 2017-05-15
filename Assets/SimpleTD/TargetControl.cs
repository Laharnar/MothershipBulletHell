using UnityEngine;
using System.Collections;
using System;

public class TargetControl : MonoBehaviour
{

    internal Transform target;
    public FireControl fire;
    public RangeCollider range;

    // Use this for initialization
    void Start()
    {
        fire = GetComponentInChildren<FireControl>();
        range = GetComponentInChildren<RangeCollider>();

    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            ReFocusAfterTargetExitsRange();
        }

    }

    private Unit GetClosestUnit()
    {
        if (range.activeDetectedUnits.Count == 0)
        {
            return null;
        }

        System.Collections.Generic.List<Unit> unitsInRange = range.GetUnits();
        Unit closestUnit = transform.GetClosestInList<Unit>(unitsInRange);
        return closestUnit;

    }


    internal Transform RequestNewTarget()
    {
        Unit unit = GetClosestUnit();
        if (unit)
        {
            return unit.transform;
        }
        return null;
    }

    internal void ReFocusAfterTargetExitsRange()
    {
        Transform old = target;
        target = RequestNewTarget();
        // remember, you dont need to check if target is null if htere arent any

        fire.SetNewTarget(target);

    }
}

public static partial class Helper
{

    public static T GetClosestInList<T>(this Transform target, System.Collections.Generic.List<T> units) where T : MonoBehaviour
    {
        float minDist = float.MaxValue;
        T closest = null;
        for (int i = 0; i < units.Count; i++)
        {
            float dist = Vector2.Distance(target.transform.position, units[i].transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = units[i];
            }
        }
        return closest;
    }
}