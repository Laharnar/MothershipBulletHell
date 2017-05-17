using UnityEngine;
using System.Collections.Generic;
using System;

class AllianceList {
    string alliance;
    List<Transform> units = new List<Transform>();
}

public static partial class UnitsInScene {

    public static List<Transform> units = new List<Transform>();
    internal static SortedList<string, List<Alliance>> unitsAsAlliances = new SortedList<string, List<Alliance>>();

    //static UnitsInScene _scene;
    internal static bool init = false;


    /*public static UnitsInScene scene {
        get
        {
            if (_scene == null)
            {
                _scene = FindObjectOfType<UnitsInScene>();
                if (_scene == null) {
                    _scene = new GameObject().AddComponent<UnitsInScene>();
                    _scene.name = "_Scene unit MANAGER";
                }
                _scene.unitsAsAlliances = new SortedList<string, List<Alliance>>();
                _scene.units = new List<Transform>();
                _scene.init = true;
            }
            return _scene;
        }
    }*/

   

    /*void Awake()
    {
        if (!init) {
            units = new List<Transform>();
            unitsAsAlliances = new SortedList<string, List<Alliance>>();
        }
    }*/

    public static void RequestNullUpdate() {
        //_scene.ClearNullShips();
        ClearNullShips();
        //m.ClearNullShips();
    }

    internal static Vector3 AimAtRandom(int x, int z, int wid, int len)
    {
        return new Vector3(UnityEngine.Random.Range(x, x+wid), UnityEngine.Random.Range(z, z + len));
    }

    public static void RegisterUnit(Transform nUnit)
    {
        units.Add(nUnit);
        Alliance flag = nUnit.GetComponent<Alliance>();
        if (!flag) {
            //Debug.Log("missing script - intentionaly?" + nUnit.name);
            return;
        }

        string key = flag.alliance;
        if (!unitsAsAlliances.ContainsKey(key))
            unitsAsAlliances.Add(key, new List<Alliance>());
        unitsAsAlliances[key].Add(nUnit.GetComponent<Alliance>());
    }

    internal static List<Alliance> GetAlliedShips(Alliance flag) {
        //if (!scene.unitsAsAlliances.ContainsKey(flag.alliance)) return new List<Alliance>();
        //return scene.unitsAsAlliances[flag.alliance];
        if (!unitsAsAlliances.ContainsKey(flag.alliance))
            return new List<Alliance>();
        return unitsAsAlliances[flag.alliance];
    }

    /// <summary>
    /// returns units sorted by distance from min to max distance
    /// </summary>
    /// <param name="position"></param>
    /// <param name="explosionRange"></param>
    /// <returns></returns>
    internal static Transform[] GetUnitsInRange(Vector3 originPosition, float radius)
    {
        List<Transform> inRange = new List<Transform>();
        List<float> ranges = new List<float>();
        for (int j = 0; j < units.Count; j++)
        {
            if (units[j] == null)
            {
                units.RemoveAt(j);
                j--;
                continue;
            }

            var unit = units[j];
            float dist = Vector3.Distance(unit.position, originPosition);
            if (dist > radius)
            {
                continue;
            }
            // insert into correct spot, sorted by range, from min to max
            bool inserted = false;
            for (int i = inRange.Count - 1; i > -1; i--)
            {
                if (ranges[i] < dist)
                {
                    ranges.Insert(i + 1, dist);
                    inRange.Insert(i + 1, unit);
                    inserted = true;
                    break;
                }
            }
            if (!inserted)
            {
                inRange.Insert(0, unit);
                ranges.Insert(0, dist);
            }
        }
        return inRange.ToArray();
    }

    internal static List<Alliance> GetAlliedShips(Alliance flag, List<AutomaticGrouping> leaders) {
        List<Alliance> allies = GetAlliedShips(flag);
        List<Alliance> isAllyLeader = new List<Alliance>();
        for (int i = 0; i < allies.Count; i++) {
            if (allies[i] == null) continue;
            bool isLeader = false;
            for (int j = 0; j < leaders.Count; j++) {
                if (leaders[j] == null) continue;
                if (leaders[j].transform == allies[i].transform) {
                    isAllyLeader.Add(allies[i]);
                    break;
                }
            }
        }
        return isAllyLeader;
    }

    internal static List<Alliance> GetEnemyShips(string flag) {
        List<Alliance> enemies = new List<Alliance>();
        for (int i = 0; i < unitsAsAlliances.Keys.Count; i++) {
            if (unitsAsAlliances.Keys[i] != flag) {
                enemies.AddRange(unitsAsAlliances[unitsAsAlliances.Keys[i]]);
            }
        }
        return enemies;
    }

    internal static List<Alliance> GetAllUnits() {
        List<Alliance> units = new List<Alliance>();
        for (int i = 0; i < unitsAsAlliances.Keys.Count; i++) {
            units.AddRange(unitsAsAlliances[unitsAsAlliances.Keys[i]]);
        }
        return units;
    }

    /// <summary>
    /// Note: doesn't return enemy, just closest ship
    /// </summary>
    /// <param name="requestBy"></param>
    /// <param name="optimizationSteps"></param>
    /// <returns></returns>
    internal static Transform GetClosestEnemyShip(Transform requestBy, int optimizationSteps) {
        if (requestBy == null) {
            Debug.LogError("Requester is null");
            return null;
        }
        ClearNullShips();
        Transform min = null;
        float mind = 0;
        for (int i = 0; i < allShips.Count; i += optimizationSteps) {
            Transform ship = allShips[i];
            if (ship.name.StartsWith("[pooling]") ||
                ship.transform == requestBy) {
                continue;
            }
            if (min == null) {
                min = allShips[i];
                if (min != null)
                    mind = Vector3.Distance(requestBy.position, min.position);
                else Debug.Log("null ships error. there are null values in allShips "+i);
                continue;
            }
            float dist = Vector3.Distance(requestBy.position, ship.position);
            if (dist < mind) {
                min = allShips[i];
                mind = dist;
            }
        }
        return min;
    }
}
