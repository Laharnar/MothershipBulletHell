using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PerfectShield : GroupControl {

    public float perfectHp = 5;
    float savedNormalHp;

    int size = 0;
    UnitInfo[,] units;

    int lastLitRing = -1;
    public int maxVibrationRings = 2;
    public int outerRing = 0;


    // lower left unit at start, for offseting indexes
    public Transform startLowerLeftUnit;
    Vector3 lowerLeft;

    public Color perfectionColor = new Color(255, 242, 138, 116);
    public Color normalColor = new Color(255, 242, 138, 116);

    public int blendingSteps = 5;

    public float rate = 0.05f;
    float time;

    public Vector2 step = Vector2.one; // increase if you have some holes between them

    // Use this for initialization
    void Start () {
        // we assume all positions are snapped
        lowerLeft = startLowerLeftUnit.localPosition;
        

        // we assume size is > 0 and size is square
        UnitInfo[] u = GetComponentsInChildren<UnitInfo>();
        size = (int)Mathf.Sqrt(u.Length);
        CreateGrid(size, u);
    }

    // Update is called once per frame
    void Update() {
        for (int i = 0; i <= 5; i++) {
            LitRing((outerRing + i) % maxVibrationRings, perfectionColor, normalColor, (float)(Math.Exp(i) / Math.Exp((float)5.0f)), false);
        }
        LitRing(0, perfectionColor, normalColor, 1, false);

        if (Time.time > time) {
            time = Time.time + rate;

            outerRing = 0 + (++outerRing % maxVibrationRings);
        }
    }

    void CreateGrid(int size, UnitInfo[] u) {
        units = new UnitInfo[size, size];
        for (int i = 0; i < u.Length; i++) {
            if (u[i] == this ||u[i] == null) {
                continue;
            }
            int x = Mathf.FloorToInt(-lowerLeft.x + u[i].transform.localPosition.x);
            int y = Mathf.FloorToInt(-lowerLeft.y + u[i].transform.localPosition.y);

            x = (int)(x / step.x);
            y = (int)(y / step.y );

            units[y, x] = u[i];

            // all units part of perfect square are tankier than usual
            savedNormalHp = units[y, x].Get<GroupUnitControl>().maxHp;
            float take = (float)Math.Sqrt(((x / (size / 2)) ^ 2 + (y / (size / 2)) ^ 2));
            units[y, x].Get<GroupUnitControl>().maxHp = take * perfectHp + (1 - take) * savedNormalHp;
            units[y, x].Get<GroupUnitControl>().hp = take * perfectHp + (1-take)*savedNormalHp;
        }
    }

    public void LitRing(int ring, Color perfectionColor, Color normalColor, float t, bool unlitPrevious = true) {
        if (lastLitRing == ring) return;

        if (unlitPrevious && lastLitRing > -1 ) {
            UnlitRing(lastLitRing, normalColor);
            lastLitRing = ring;
        }

        t = Mathf.Clamp(t, 0.0f, 1.0f);
        Color blend = new Color((float)perfectionColor.r * t + (float)normalColor.r * (1 - t),
                                (float)perfectionColor.g * t + (float)normalColor.g * (1 - t),
                                (float)perfectionColor.b * t + (float)normalColor.b * (1 - t),
                                (float)perfectionColor.a * t + (float)normalColor.a * (1 - t));
        int range = this.size- ring;
        for (int i = ring; i < range; i++) {
            for (int j = ring; j < range; j++) {
                if (i == ring || j == ring || i == range-1 || j == range-1 )
                    if (units[i, j] != null)
                        units[i, j].GetCompt<SpriteRenderer>().color = blend;
            }
        }
    }

    void SetHpToRing(int ring, float hp) {
        // Note: hp can't be more than perfect hp, since its limited by max hp
        int range = this.size - ring;
        for (int i = ring; i < range; i++) {
            for (int j = ring; j < range; j++) {
                if ((i == ring || j == ring || i == range - 1 || j == range - 1) && units[i, j] != null)
                    units[i, j].Get<GroupUnitControl>().hp = hp;
            }
        }
    }

    public void UnlitRing(int ring, Color normalColor) {
        int range = this.size - ring;
        for (int i = ring; i < range; i++) {
            for (int j = ring; j < range; j++) {
                if ((i == ring || j == ring || i == range - 1 || j == range - 1)&& units[i, j] != null)
                    units[i, j].GetCompt<SpriteRenderer>().color = normalColor;
            }
        }
    }

    public void DispatchRing(int ring) {
        SetHpToRing(ring, savedNormalHp);
        UnlitRing(ring, normalColor);
        // send unwanted squares off
        UnitInfo[] infos = new UnitInfo[(size-2)*(size-2)];
        int range = this.size - ring;
        for (int i = 0; i < members.Count; i++) {
            if (members[i] == null) continue;
            int x = Mathf.FloorToInt(-lowerLeft.x + members[i].transform.localPosition.x);
            int y = Mathf.FloorToInt(-lowerLeft.y + members[i].transform.localPosition.y);
            if (x == ring || y == ring || x == range - 1 || y == range - 1) {
                EnableMember(i, true);
                UnRegister(i);
            }
        }
        //this.size -= 2;
        // recreate grid
        /*lowerLeft = new Vector3(lowerLeft.x+1, lowerLeft.y+1, 0);
        units = new UnitInfo[size, size];
        for (int i = 0; i < members.Count; i++) {
            int x = Mathf.FloorToInt(-lowerLeft.x + members[i].transform.localPosition.x);
            int y = Mathf.FloorToInt(-lowerLeft.y + members[i].transform.localPosition.y);
            Debug.Log(y + " " + x + " "+size);
            units[y, x] = members[i].info;

            // all units part of perfect square are tankier than usual
            units[y, x].Get<GroupUnitControl>(typeof(HpControl)).hp = perfectHp;
        }*/
    }


    private void BlendLit(int ring, Color perfectionColor, Color normalColor, int steps) {
        for (int i = 0; i <= steps; i++) {
            LitRing((outerRing + i) % maxVibrationRings, perfectionColor, normalColor, (float)(Math.Exp(i) / Math.Exp((float)steps)), false);
        }
    }

    public override void OnMemberDestroyed(GroupUnitControl destroyedMember) {
        if (!enabled) return;
        //Debug.Log("member down");
        base.OnMemberDestroyed(destroyedMember);

        //DispatchRing(0);
    }
}
