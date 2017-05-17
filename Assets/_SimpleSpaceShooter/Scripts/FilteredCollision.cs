using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Handles flag based masking for allies, your faction, enemies and utility.
/// Put this script on both damagers receivers. Also add ProxyCollision somewhere and link it to this script
/// 
/// Dont forget to add trigger collider on same object and Unit info somewhere on parent
/// </summary>
// Note: can't collide on its own, its just a filter
// Note: assumes ally and enemy are always at least damage collisions
public abstract class FilteredCollision : MonoBehaviour {

    public string flag;// ex:"player"
    public string[] moreAllies; // ex:"player2"
    public string[] utilityCollide; // which  _utility  must collide with // TODO: export this into a single manager script. then link to manager via flag. each flag should have a alist of enemies
    public string[] projectileCollide; // which  _utility  must collide with // TODO: export this into a single manager script. then link to manager via flag. each flag should have a alist of enemies

    // Note that 2 static objects cant collide, or there will be a loop.
    public bool staticObject = false; // you must send collisions to static object via moving ones

    public bool isUtility = false;
    public bool isUnit = false; // is object with this script specificaly, a unit
    public bool isProjectile = false;

    /// <summary>
    /// Filters unwanted collisions and resends them to their type
    /// - self, ally, enemy, utility
    /// </summary>
    /// <param name="other"></param>
    /// <returns>False: collision isnt allowed. True: collision is allowed</returns>
    protected bool Filter(FilteredCollision collision, out int selectedFilter) {
        
        selectedFilter = -1;
        if (collision == null) return false;

        //Debug.Log("filter : " + collision + " " + IsMatchingFlag(collision.flag, utilityCollide), this);

        if (collision.flag == flag) selectedFilter = 0; // use for healing
        else if (IsMatchingFlag(collision.flag, moreAllies)) selectedFilter = 1; // ignore damage on allies
        else if (collision.isUtility && IsMatchingFlag(collision.flag, utilityCollide)) selectedFilter = 2;
        else if (collision.isProjectile) selectedFilter = 3;
        else if (collision.isUnit) selectedFilter = 4; // damage enemies
        else {
            if (isUtility && !IsMatchingFlag(collision.flag, utilityCollide))
                Debug.Log("utility collision, not matching " + flag + " "+collision.flag);
            //else Debug.Log("ignoring collision "+ name + ": " +flag+ " " + collision.name + " "+collision.flag + 
              //  " "+ collision.isUtility + " "+IsMatchingFlag(collision.flag, utilityCollide), this);
            return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if flag is of some allied faction, NOT your own;
    /// </summary>
    /// <param name="flag"></param>
    /// <returns></returns>
    private bool IsMatchingFlag(string lookForFlag, string[] flags) {
        foreach (var checkFlag in flags) {
            if (checkFlag == lookForFlag) {
                return true;
            }
        }
        return false;
    }

}
