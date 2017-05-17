using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public abstract class AiBase : TargetableBase{
    // alliance/ai
    protected DecisionLeaf root;

    public DecisionLeaf GetLogic() {
        return root;
    }
}

/// <summary>
/// Base for root for target searching.
/// Dont use it on children, if you want to set target to some child of root, use list of child collision triggers instead.
/// </summary>
public abstract class TargetableBase : PooledMonoBehaviour {

    /// <summary>
    /// Simple reference to source
    /// </summary>
    public UnitInfo targetingShipSource;

    public static Dictionary<string, List<UnitInfo>> allTargets = new Dictionary<string, List<UnitInfo>>();
    public string initAlliance;

    /// <summary>
    /// This defines who many guns at maximum should follow this target.
    /// </summary>
    //public int targetingPriority = 1;

    protected void Awake() {
        if (initAlliance == null) Debug.Log("Set alliance, it's empty.", this);


        if (targetingShipSource == null) {
            Debug.Log("Set source.", this);
            return;
        }

        // registed target into list of all targets in scene.
        if (!allTargets.ContainsKey(initAlliance))
            allTargets.Add(initAlliance, new List<UnitInfo>());
        allTargets[initAlliance].Add(targetingShipSource);
    }
}