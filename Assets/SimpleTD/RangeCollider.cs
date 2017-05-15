using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RangeCollider : MonoBehaviour {

    public List<Unit> activeDetectedUnits;

    public List<Unit> allUnitsDetected;

    [SerializeField]internal ExitRange onExitRange;

    // Use this for initialization
    void Start () {
        activeDetectedUnits = new List<Unit>();
        allUnitsDetected = new List<Unit>();
        if (!GetComponent<Collider2D>().isTrigger) {
            Debug.LogError("ERROR!!!!");
        }
	}
	
	// Update is called once per frame
	void OnTriggerEnter2D (Collider2D other) {
        Unit detectedUnit = other.GetComponent<Unit>();
        if (detectedUnit) {
            if (activeDetectedUnits.Contains(detectedUnit)) {
                Debug.LogWarning("double unit detection, source: "+this+ " other:"+ detectedUnit);
            }
            else activeDetectedUnits.Add(detectedUnit);
        }
	}
    
    void OnTriggerExit2D(Collider2D other) {
        Unit lostUnit = other.GetComponent<Unit>();
        if (lostUnit) {
            activeDetectedUnits.Remove(lostUnit);
            onExitRange.Exit(lostUnit.transform);

            allUnitsDetected.RemoveAll(allU => allU == null);
            if (allUnitsDetected.Contains(lostUnit)) {
                Debug.LogWarning("double unit lost, source: " + this + " other:" + lostUnit);
            } else allUnitsDetected.Add(lostUnit);
        }
    }

    internal List<Unit> GetUnits() {
        //activeDetectedUnits = Helper.ClearNulls<Unit>(ref activeDetectedUnits);
        activeDetectedUnits.RemoveAll(unit => unit == null);
        return activeDetectedUnits;
    }

    
}

[System.Serializable]
class ExitRange
{
    public TargetControl targeting;
    [SerializeField]Transform lastExitUnit;

    public void Exit(Transform lostUnit)
    {
        if (targeting.target.transform == lostUnit.transform && lostUnit != null)
        {
            targeting.ReFocusAfterTargetExitsRange();
            lastExitUnit = lostUnit.transform;
        }
    }
}

public static partial class Helper {
    public static void ClearNulls<T>(List<T> list) {
        list.RemoveAll(item => item == null);
    }
}