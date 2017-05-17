using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// Supports group movement via assigning his group where to go
/// Group must be assigne manualy for now
/// </summary>
public class Group : MonoBehaviour {

    List<SimpleGroupUnit> units = new List<SimpleGroupUnit>();
    string groupType = "";
    float groupDefaultSpeed = 1;

    LineFormation formation;
    internal SimpleGroupUnit leader;

    // Use this for initialization
    void Start () {
        formation = GetComponent<LineFormation>();
    }

    // Update is called once per frame
    void Update () {
        if (units.Count == 0)
        {
            return;
        }
        SimpleGroupUnit leader = units[0];
        SetLeader(units[0]);
        leader.GetComponent<SpriteRenderer>().material.color = Color.red;
        leader.GroupMoveTo(new Vector3(10, 80, 0), units, formation);
        Debug.DrawRay(leader.transform.position, leader.transform.right * 10, Color.grey);
        Debug.DrawRay(leader.transform.position, -leader.transform.right * 10, Color.grey);
	}

    void SetLeader(SimpleGroupUnit unit)
    {
        this.leader = unit;
    }

    internal void RegisterUnit(SimpleGroupUnit simpleGroupUnit)
    {
        // first unit in group will define type of group
        if (units.Count == 0)
        {
            groupType = simpleGroupUnit.unitType;
            groupDefaultSpeed = simpleGroupUnit.speed;
        }
        // add units, filter units with wrong types
        if (simpleGroupUnit.unitType == groupType)
        {
            units.Add(simpleGroupUnit);
        }
    }
}
