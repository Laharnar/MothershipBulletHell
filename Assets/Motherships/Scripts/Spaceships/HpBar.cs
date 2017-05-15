//#define usingMothershipVersion

#if usingMothershipVersion
using UnityEngine;
using System.Collections;

public class HpBar : MonoBehaviour {

    /// <summary>
    /// Ship that's connected to this hp bar.
    /// </summary>
    public Spaceship ship;

    /// <summary>
    /// Gui object for this bar.
    /// </summary>
    public SpriteRenderer bar;

    Vector3 originalScale;
    Vector3 originalPosition;

    void Start() {
        originalScale = bar.transform.localScale;
        originalPosition = bar.transform.position;

        ship.RegisterShipHpStatus(UpdateBarOnHpChange);
    }

    void UpdateBarOnHpChange(float health) {
        var scale = originalScale.x* health/ship.maxHealth;
        //transform.position -= new Vector3((int)scale, 0 , 0);

        bar.transform.localScale = new Vector3(scale, originalScale.y, originalScale.z);
    }
}
#else 

/*
Hp bar script that uses simple sprite and objects scale

How to create hp bar:
1) Create new sprite via Create/Sprite/Square, or make your own image
2) Assign it as sprite renderer on empty game object
3) Connect it to data with max hp, which is needed for size calculations

Note that object with this  is scaled so script should be on child object.
*/
using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public partial class HpBar : MonoBehaviour
{

    /// <summary>
    /// Hp data that's connected to this hp bar.
    /// HpControl needs .hp float value
    /// </summary>
    public HpControl hpShieldData;

    public Bar bar_hp;
    public Bar bar_shield;

    // player might get destroyed and hp wont be updated anymore
    public bool setHpToZeroWithoutSource = true;

    // by default, hp is visualy reduced to the left
    public bool reduceHpToCenter = false;

    void Start()
    {
        bar_hp.SaveOrig(this);
        bar_shield.SaveOrig(this);

        if (bar_hp.useBar == false && bar_shield.useBar == false ) {
            Debug.LogError("Unused HpBar script, consider removing it", this);
        }
    }

    void Update()
    {
        UpdateBar(bar_hp, hpShieldData.hp, hpShieldData.maxHp);
        UpdateBar(bar_shield, hpShieldData.shield, hpShieldData.maxShields);
    }

    void OnDestroy() {
        Update();
    }

    internal void UpdateBar(Bar bar, float value, float maxValue)
    {
        if (!bar.useBar) {
            return;
        }

        if (bar.bar) {
            var scale = bar.originalScale.x * (value / maxValue);

            // scale bar to left
            if (!reduceHpToCenter) {
                var posx = bar.originalPosition.x - (bar.originalScale.x - scale) / 2;
                bar.bar.transform.localPosition = new Vector3(posx, bar.originalPosition.y, 0);
            }

            bar.bar.transform.localScale = new Vector3(scale, bar.originalScale.y, bar.originalScale.z);
        }
        if (bar.rawBar) {
            var scale = bar.originalScaleRaw.x * (value / maxValue);

            // scale bar to left
            if (!reduceHpToCenter) {
                var posx = bar.originalPositionRaw.x - (bar.originalScaleRaw.x - bar.rawBar.rectTransform.rect.width) / 2;
                bar.rawBar.transform.localPosition = new Vector3(posx, bar.originalPositionRaw.y, 0);
            }

            bar.rawBar.transform.localScale = new Vector3(scale, bar.originalScaleRaw.y, bar.originalScaleRaw.z);
        }
    }
}

[System.Serializable]
public class Bar {
    public SpriteRenderer bar;
    public RawImage rawBar;
    public string barName;
    public bool useBar = false;
    internal Vector3 originalScale;
    internal Vector3 originalPosition;


    internal Vector3 originalScaleRaw;
    internal Vector3 originalPositionRaw;

    internal void SaveOrig(HpBar thisTransform) {
        if (!useBar) {
            return;
        }
        if (bar == null && rawBar == null) Debug.LogError("Bar hp sprite isnt assigned.", thisTransform);
        if (bar) {
            originalPosition = bar.transform.localPosition;
            originalScale = bar.transform.localScale;
        }
        if (rawBar) {
            originalPositionRaw = rawBar.rectTransform.anchoredPosition;
            originalScaleRaw = rawBar.transform.localScale;
        }
    }
}
#endif