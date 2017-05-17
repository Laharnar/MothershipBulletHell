using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// Included follow target behaviour on top of SimpleUnit.
/// 
/// Depends on some kind of UnitsInScene list
/// How to use: just set the target
/// </summary>
[RequireComponent(typeof(SimpleUnit))]
public class SceneDependantAI : MonoBehaviour
{

    /// <summary>
    /// Object that will rotate/move.
    /// </summary>
    public SimpleUnit getSource;
    public Vector3 upVector;
    //public UnitsInScene scene;

    string state = "idle";

    public float rotationAccuracy = 3;

    internal Vector3 AimAtRandom()
    {
        return UnitsInScene.AimAtRandom(-100, -100, 200, 200);
    }

    public float slowDownEffect = 2;
    public float innerAccuracy = 0.1f;
    public bool drawLine = true;

    // Use this for initialization
    void Awake()
    {
        if (!getSource) getSource = GetComponent<SimpleUnit>();

        //if (!UnitsInScene) UnitsInScene = GameObject.FindObjectOfType<UnitsInScene>();
        //if (UnitsInScene)
            UnitsInScene.RegisterUnit(transform);
        //else Logger.AddLog("Missing Units in scene manager.", this);
        //StartCoroutine(DistanceBasedAI());
    }

    /// <summary>
    /// EXAMPLE BEHAVIOUR
    /// Note: new version isn't tested.
    /// </summary>
    /// <returns></returns>
    /*[System.Obsolete("This is default behaviour. At the moment it's disabled(Start function).")]
    public IEnumerator DistanceBasedAI()
    {
        yield return Follow(ChoseTarget(), false);
    }*/

   public IEnumerator Follow(Transform target, bool stopWitoutTarget, float howNear = 0.1f, Func<Transform, float> distanceCalculation = null)
    {
        while (target)
        {
            SceneSearching.GoTo(transform, getSource, target);
            // use custom or default distance calculation
            if (!((distanceCalculation != null && distanceCalculation(target) > howNear)
                || (distanceCalculation == null && DefaultDistance(target.position) > howNear)))
                break;
            yield return null;
        }
        // go idle, dont reset movement if it has to go forward
        getSource.steering = getSource.nonModifiedSteering;
        getSource.Idle(resetMovement: stopWitoutTarget);
    }

    internal float DefaultDistance(Vector3 target)
    {
        return Vector3.Distance(transform.position, target);
    }
    
    public Transform ChoseTarget()
    {
        Transform target = null;
        List<Transform> units = UnitsInScene.units;
        for (int i = 0; i < units.Count; i++)
        {
            if (units[i] == transform)
            {
                continue;
            }
            target = units[i].transform;
            break;
        }
        return target;
    }

    public Transform ChoseClosestTarget(Transform transform, string flag)
    {
        List<Alliance> enemies = UnitsInScene.GetEnemyShips(flag);
        float minDist = float.MaxValue;
        int imin = -1;
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] == null)
            {
                continue;
            }
            if (enemies[i].alliance != flag)
            {
                if (Vector3.Distance(enemies[i].transform.position, transform.position)
                    < minDist)
                {
                    imin = i;
                }
            }
        }
        if (imin != -1)
        {
            return enemies[imin].transform;
        }
        return null;
    }

    internal void Idle(
        bool instaResetMov = false, bool instaResetRot = true,
        bool resetMovement = true, bool resetRotation = true)
    {
        getSource.Idle(instaResetMov, instaResetRot, resetMovement, resetRotation);
    }

    /*[System.Obsolete("not used anywhere?")]
    void GoTo(Vector3 target, Vector3 targetUp)
    {
        transform.TurnTowards(this, target);
        //TurnTowards(target, targetUp);
        getSource.ExecuteMove(SimpleUnit.Moves.forward);
    }*/
}
