using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// *10.5.2017
/// Changed into static version, instead of singleton manager.
/// 
/// </summary>
//[System.Obsolete("Incorrect aproach, use other version")]
public static partial class UnitsInScene {

    //public static UnitsInScene m { get { return scene; } }

    static List<Transform> allShips { get { return units; } }

    private static int approxAverageNumOfShipsPerScan;// not true average, since search can exit early if big group is suddenly found
    
    public static void RegisterUnits(params Transform[] units) {
        if (units.Length > 0) {
            foreach (var unit in units) {
                RegisterUnit(unit);
                //allShips.Add(unit);                
            }
        }
    }

    /*public static List<Transform> GetAllShips() {
        return allShips;
    }*/
   
    private static void ClearNullShips() {
        for (int i = 0; i < allShips.Count; i++) {
            if (allShips[i] == null) {
                allShips.RemoveAt(i);
                i--;
            }
        }
    }

    /// <summary>
    /// It goes like this: scan 1 ship, assign all ships in range to it and remove them from next scans
    /// Repeat for all and return array with all ranges
    /// </summary>
    /// <param name="blastRadius">Put your explosion radius here</param>
    public static Transform GetApproximationOfGroupCount(MonoBehaviour requestedBy, float blastRadius = 100)
    {
        ClearNullShips();

        List<Transform> shipsNotScanned = new List<Transform>(allShips);

        bool[] ignore = new bool[shipsNotScanned.Count];

        // use statistics to quick chose targets
        int maxCount = 0;
        Transform maxCountShip = null;
        int sum = 0;
        int count = 0;
        for (int i = 0; i < shipsNotScanned.Count; i++) {
            if (ignore[i]) {
                continue;
            }
            Transform unit = shipsNotScanned[i];
            //print(unit);
            //Debug.DrawRay(unit.position, Vector2.up * blastRadius, Color.cyan, 5);
            Collider2D[] results = Physics2D.OverlapCircleAll(unit.position, blastRadius, 1 << LayerMask.NameToLayer("Detection"));
            int unitCount = results.Length;
            //print(allShips.Count + " "+requestedBy + " " + results.Length + " " + unitCount);

            count++;
            sum += unitCount;

            for (int j = 0; j < results.Length; j++) {
                int index = allShips.IndexOf(results[j].transform.root);
                if (index > -1 && !ignore[index] && allShips.Contains(results[j].transform.root)) {
                    //print(requestedBy + " " + results[j] + " " + results[j].transform.root);
                    ignore[index] = true;
                }
            }

            if (unitCount > maxCount) {
                maxCount = unitCount;
                maxCountShip = unit;
            }

            // early exit if optimal group is found. this will lead to towers shooting to the group 1 or 2 times before avg will increase and search will broaden
            if (approxAverageNumOfShipsPerScan > 0 && unitCount / 2 > approxAverageNumOfShipsPerScan) {
                break;
            }
        }

        if (maxCountShip == null) {
            return null;
        }

        approxAverageNumOfShipsPerScan = sum / count;
        return maxCountShip;
    }


    internal static Transform GetClosestShip(Transform requestBy, int optimizationSteps) {
        if (requestBy == null) {
            Debug.LogError("Requester is null");
            return null;
        }
        ClearNullShips();
        Transform min = null;
        float mind = 0;
        for (int i = 0; i < allShips.Count; i+=optimizationSteps) {
            Transform ship = allShips[i];
            if (ship.name.StartsWith("[pooling]")) {
                continue;
            }
            if (min == null) {
                min = allShips[i];
                if (min != null)
                    mind = Vector3.Distance(requestBy.position, min.position);
                else Debug.Log("null ships error.");
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
