using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LevelEnd))]
public class LevelHp : HpControl {


    LevelEnd endMenus;

    void Start()
    {
        destroyWhenHpIsZero = false;
        endMenus = GetComponent<LevelEnd>();

        base.Start();
    }

    protected override void HpZero()
    {
        endMenus.FinishedLevelByLosingAllHp();
    }
}
