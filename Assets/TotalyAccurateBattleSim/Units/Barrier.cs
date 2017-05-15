using UnityEngine;
using System.Collections;
using System;

public class Barrier : HpControl
{
    // change transparency depending on shield left 
    // - use for fade effect
    public SpriteRenderer sprite_shield;
    Color setCol_shield;
    float maxTransparency_shield; // set max value to alpha

    // change transparency depending on hp left 
    // - use for burn effect
    public SpriteRenderer sprite_hp;
    Color setCol_hp;
    float maxTransparency_hp; // set max value to alpha


    new void Awake()
    {
        if (sprite_shield)
        {
            setCol_shield = sprite_shield.color;
            maxTransparency_shield = setCol_shield.a;
        }
        if (sprite_hp)
        {
            setCol_hp = sprite_shield.color;
            maxTransparency_hp = setCol_shield.a;
        }
    }

    // Use this for initialization
    void Start()
    {
        base.Start();

        destroyWhenHpIsZero = false;
    }

    // fade in
    protected override void HpUpdate()
    {
        if (!sprite_hp)
        {
            return;
        }
        // apply fire effect to ship to burn
        setCol_hp.a = Mathf.Clamp(hp/maxHp - (maxTransparency_hp), 0, 1);
        sprite_hp.color = setCol_hp;
    }

    // fades out
    protected override void ShieldUpdate()
    {
        if (!sprite_shield)
        {
            return;
        }
        // change shield's transparency depending on hp left
        setCol_shield.a = Mathf.Clamp(hp / maxShields - (1 - maxTransparency_shield), 0, 1);
        sprite_shield.color = setCol_shield;
    }
}
