using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Drones attack closest enemy and instantly pop all drones around them and they do the same, causing chain reactions.
/// Normal units just get damage.
/// </summary>
[RequireComponent(typeof(SceneDependantAI))]
[RequireComponent(typeof(Alliance))]
[RequireComponent(typeof(StructuredUnit))]
public class Drone : MonoBehaviour
{
    SceneDependantAI followAi;

    // put drones around this one on a stack that explodes them
    DroneExplosionList mExplosionStack;

    public int droneDamage = 1;

    Alliance flag;

    StructuredUnit structureOfUnit;
    float additionalSizeParam; // sum of aproximated sizes of drone and its target, for distance calculation

    public bool chainDestroy = true; // is units included in chains
    private float explosionRadius = 2;

    public bool stopWithoutTarget = false;
    
    void Start()
    {
        structureOfUnit = GetComponent<StructuredUnit>();
        followAi = GetComponent<SceneDependantAI>();
        flag = GetComponent<Alliance>();

        mExplosionStack = GameObject.FindObjectOfType<DroneExplosionList>();

        StartCoroutine(DrawExplosionRadius());

        StartCoroutine(DroneBehaviour());
    }

    internal void GetUnitsAroundAndRegisterThem(int depth)
    {
        if (chainDestroy)
            ExplodeDmg(depth);
    }

    private IEnumerator DrawExplosionRadius()
    {
        while (true)
        {
            Debug.DrawRay(transform.position + Vector3.down * explosionRadius, 2 * Vector2.up * explosionRadius, Color.yellow, 0.5f);
            Debug.DrawRay(transform.position + Vector3.left * explosionRadius, 2 * Vector2.right * explosionRadius, Color.yellow, 0.5f);
            yield return new WaitForSeconds(1);
        }
    }

    // Update is called once per frame
    IEnumerator DroneBehaviour()
    {
        Transform target = followAi.ChoseClosestTarget(transform, flag.alliance);
        if (target)
        { 
            additionalSizeParam = structureOfUnit.approximatedSize + target.GetComponent<StructuredUnit>().approximatedSize;

            yield return followAi.Follow(target, stopWithoutTarget, 0.2f, DistanceWithSize);
            
            //ExplodeOrSmth();
            if (target)
            {
                ExplodeDmg();
            }
        }
    }

    public void ExplodeDmg(int depth = 0)
    {
        Transform[] unitsInRange = UnitsInScene.GetUnitsInRange(transform.position,
            explosionRadius);
        RegisterChainExplosion(unitsInRange, depth);
        GetComponent<SpriteRenderer>().color = Color.green;
    }
    /// <summary>
    /// Drones instantly pop all drones around them and they do the same.
    /// Normal units just get damage.
    /// </summary>
    /// <param name="applyDmgTo"></param>
    /// <returns></returns>
    private void RegisterChainExplosion(Transform[] applyDmgTo, int depth)
    {
        for (int i = 0; i < applyDmgTo.Length; i++)
        {
            if (!applyDmgTo[i])  continue;
            
            // drone ally/enemy
            Drone isDrone = applyDmgTo[i].GetComponent<Drone>();
            if (isDrone)
            {
                // put this drone and other nearby drones on explosion stack
                mExplosionStack.RegisterDrone(depth, isDrone);
            }
            // normal enemy
            else
            {
                applyDmgTo[i].GetComponent<HpControl>().Damage(droneDamage);
            }
        }
        Debug.Log("exploding, nearby: "+applyDmgTo.Length );
    }
    
    public float DistanceWithSize(Transform target)
    {
        return Vector3.Distance(transform.position, target.position)-additionalSizeParam;
    }
}
