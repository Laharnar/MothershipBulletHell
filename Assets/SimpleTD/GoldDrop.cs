using UnityEngine;
using System.Collections;

public class GoldDrop : HpControl {

    public int worthGold = 1;

    public void Drop()
    {
        GoldCollected.AddGold(worthGold);
    }

    protected override void HpZero()
    {
        Drop();
    }
}
