using UnityEngine;
using System.Collections;

public class RepairRegen : MonoBehaviour {

    public HpControl hpData;

    public float regenPerRate_hp = 1;
    public float regenPerRate_shield = 1;

    public float regenRate_hp = 0.75f;
    public float regenRate_shield = 0.75f;

    public bool startAtMax_hp = true;
    public bool startAtMax_shield = false;


    // Use this for initialization
    void Start () {
        if (!startAtMax_hp)
            hpData.hp = 0;

        if (!startAtMax_shield)
            hpData.shield = 0;

        StartCoroutine(HpRegen());
        StartCoroutine(ShieldRegen());
    }

    private IEnumerator ShieldRegen()
    {
        float rate = regenRate_shield;// rate: 1 sec
        float t = Time.time + rate;
        while (true)
        {
            if (Time.time > t)
            {
                t = Time.time + rate;
                hpData.shield += regenPerRate_shield;
            }
            yield return null;
        }
    }

    private IEnumerator HpRegen()
    {
        float rate = regenRate_hp;// rate: 1 sec
        float t = Time.time + rate;
        while (true)
        {
            if (Time.time > t)
            {
                t = Time.time + rate;
                hpData.hp += regenPerRate_hp;
            }
            yield return null;
        }
    }
}
