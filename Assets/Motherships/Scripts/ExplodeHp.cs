using UnityEngine;
using System.Collections;

/// <summary>
/// Slowly explodes, firing animation
/// </summary>
public class ExplodeHp : HpControl {

    public System.Action onDestroyed;

    protected override void HpZero()
    {
        if (onDestroyed != null)
            onDestroyed();

        //GetComponent<CircleCollider2D>().enabled = false;
        GetComponent<SimpleUnit>().Idle(resetMovement: false, instantlyResetRot: false);
        GetComponent<SimpleUnit>().Idle(resetMovement: false, instantlyResetRot: false);
    }
}
