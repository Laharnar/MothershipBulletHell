using UnityEngine;
using System.Collections;

/// <summary>
/// Provides interface between scrap acessing and dropping.
/// 
/// Date added: 19.8.2016
/// 
/// Edit 2.0:
/// Now it acts as singleton manager, that adds scrap via static function, that ensures no errors(logs warning) if script isn't in scene.
/// </summary>
/// <remarks>
/// Better approach would be to have display manager which would process data, scrap maanger which would hold data, and text displays connected to display manager and would receive text from it.
/// </remarks>
public class ScrapManager : MonoBehaviour {

    static ScrapManager m;

    /// <summary>
    /// Drop/scrap that player collected so far.
    /// </summary>
    internal int collectedDrop;

    public UpgradeText[] upgradeButtons;

    public int CollectedScrap { get { return collectedDrop; } }

    void Start() {
        m = this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dropAmount"></param>
    void AddScrap(ScrapDrop origin, int dropAmount) {
        this.collectedDrop += dropAmount;

        for (int i = 0; i < upgradeButtons.Length; i++) {
            if (upgradeButtons[i].gameObject.activeInHierarchy) {
                upgradeButtons[i].UpdateDisplay();
            }
        }
    }


    internal void UseScrap(int amount) {
        this.collectedDrop -= amount;
    }

    internal static void RegisterScrapDrop(ScrapDrop scrapDrop, int dropAmount) {
        if (m) {
            m.AddScrap(scrapDrop, dropAmount);
        } else {
            Debug.Log("SCRIPT ISN'T IN SCENE. CAPS");
            UnityConsole.WriteWarning("<bold color='red'>ScrapManager</color> script isn't in scene");
        }
    }
}
