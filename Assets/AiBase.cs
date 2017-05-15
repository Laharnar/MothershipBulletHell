using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AiBase : PooledMonoBehaviour {
    // alliance/ai
    public static Dictionary<string, List<UnitInfo>> allAi = new Dictionary<string, List<UnitInfo>>();
    public string initAlliance;

    public DecisionLeaf GetLogic() {
        return root;
    }

    protected DecisionLeaf root;

    protected void Awake() {
        if (!allAi.ContainsKey(initAlliance))
            allAi.Add(initAlliance, new List<UnitInfo>());
        allAi[initAlliance].Add(GetComponent<UnitInfo>());
    }
}