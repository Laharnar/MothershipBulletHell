using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Handles win/lose menus
/// </summary>
public class LevelEnd : MonoBehaviour {

    public Transform winByRunningOutOfEnemiesMenu;
    public Transform loseByRunningOutOfHpMenu;

    internal void FinishedLevelByRunningOutOfEnemies()
    {
        Open(winByRunningOutOfEnemiesMenu);
    }

    internal void FinishedLevelByLosingAllHp()
    {
        Open(loseByRunningOutOfHpMenu);
    }

    // Note: menus should be closed via "close on startup" script
    private void Open(Transform menu)
    {
        if(menu)
            menu.gameObject.SetActive(true);
        else Debug.LogWarning("null menu");
    }
}
