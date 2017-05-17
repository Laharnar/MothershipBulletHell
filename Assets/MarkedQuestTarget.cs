using UnityEngine;
using System.Collections;

/// <summary>
/// Marked Destructible target for quest. It callbacks to the source quest.
/// Don't forget to init it.
/// </summary>
public class MarkedQuestTarget : MonoBehaviour {
    // save quest for later when this object will get destroyed
    // invoke callback on it
    DestroyQuest targetQuest;

    public void Init(DestroyQuest q) {
        targetQuest = q;
    }

    private void OnDestroy() {
        if (targetQuest != null)
            targetQuest.Completed();
        else Debug.Log("err");
    }
}