using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


[System.Serializable]
public class FormationData {
    public int width, height;
    // Indexes, where formation exists at given width and height. Leader is random
    public int[] formation;
    public int maxSize = 0;

    public Vector2 step; // step in calculation positions 

    public List<AutomaticGrouping> assignedChildren = new List<AutomaticGrouping>();

    public FormationData(int width, int height, Vector2 step, float time) {
        width = Mathf.Clamp(width, 1, int.MaxValue);
        height = Mathf.Clamp(height, 1, int.MaxValue);

        this.width = width;
        this.height = height;
        this.step = step;

        UpdateSize(time);
    }

    public void UpdateSize(float countedTime) {
        maxSize = Mathf.FloorToInt(countedTime);
        formation = WingIntTable(width, height, maxSize);//RandIntTable(width, height, nUnits);
    }

    /// <summary>
    /// Random locations in set widht and height
    /// </summary>
    /// <param name="maxwidth">min 1!!!</param>
    /// <param name="maxheight">min 1!!!</param>
    /// <param name="nUnits"></param>
    /// <returns></returns>
    private int[] RandIntTable(int maxwidth, int maxheight, int nUnits) {
        int[] a = new int[nUnits];
        for (int i = 0; i < nUnits; i++) {
            a[i] = (UnityEngine.Random.Range(0, maxheight - 1) * maxwidth
                + UnityEngine.Random.Range(0, maxwidth - 1)
                );// todo:formul is wrong, should go 
        }
        return a;
    }

    /// <summary>
    /// -       -
    ///   -   -
    ///     -
    /// </summary>
    /// <param name="maxwidth"></param>
    /// <param name="maxheight"></param>
    /// <param name="nUnits"></param>
    /// <returns></returns>
    private int[] WingIntTable(int maxwidth, int maxheight, int nUnits) {
        int[] a = new int[nUnits];
        for (int i = 0; i < nUnits; i++) {
            a[i] = (i - i % width) * width + i;
        }
        return a;
    }

    /// <summary>
    /// THIS VERSION DOESN'T ROTATE THE FORMATION WITH LEADER.
    /// </summary>
    /// <param name="leader"></param>
    /// <param name="units"></param>
    internal void ApplyFormation(AutomaticGrouping leader, List<AutomaticGrouping> units) {
        for (int i = 0; i < formation.Length && i < units.Count; i++) {
            units[i].bomber_v2.travelTo.secondaryTravel = (leader.transform.position + GetPoint(i));
            units[i].bomber_v2.travelTo.leader = leader;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="i">index of plane in formation table</param>
    /// <returns></returns>
    private Vector3 GetPoint(int i) {
        int k = formation[i];
        int x = k % width;
        int y = (k - x) / width;
        return new Vector3(x * step.x, y * step.y);
    }

    public void AddChild(AutomaticGrouping unit) {
        if (!isFull)
            assignedChildren.Add(unit);
        // Note: Maybe add some backward connection, from unit to leader?
        else Debug.Log("fix, cant happen");
    }

    public bool isFull {
        get {
            return assignedChildren.Count == maxSize;
        }
    }
}

public class FormationManager : MonoBehaviour {
    //todo: include alliance based grouping. every alliance should have its own grouping
    // implement this directly in units in scene?

    static FormationManager _scene;
    public static FormationManager scene {
        get {
            
            if (_scene == null) {
                _scene = GameObject.FindObjectOfType<FormationManager>();
                if (_scene == null) {
                    GameObject m = new GameObject();
                    m.name = "FormationManager";
                    _scene = m.AddComponent<FormationManager>();
                    
                }
            }
            return _scene;
        }
    }

    List<AutomaticGrouping> leaders = new List<AutomaticGrouping>();
    // all units, leaders included. it should allow formations to be formed within formation
    List<AutomaticGrouping> groupedUnits = new List<AutomaticGrouping>();

    // x, z: x, widht, y, w: y, height
    public Vector4 minMaxWidthHeight;
    public Vector4 minMaxStep;


    FormationData CreateFormation(float countedTime) {
        float r = UnityEngine.Random.Range(0f, 1f);
        float width = (int)minMaxWidthHeight.x * (1 - r) + (int)minMaxWidthHeight.z * r;
        r = UnityEngine.Random.Range(0f, 1f);
        float height = (int)minMaxWidthHeight.y * (1 - r) + (int)minMaxWidthHeight.w * r;
        r = UnityEngine.Random.Range(0f, 1f);
        float stepx = (int)minMaxWidthHeight.x * (1 - r) + (int)minMaxWidthHeight.z * r;
        r = UnityEngine.Random.Range(0f, 1f);
        float stepy = (int)minMaxWidthHeight.y * (1 - r) + (int)minMaxWidthHeight.w * r;
        
        return new FormationData(Mathf.FloorToInt(width), Mathf.FloorToInt(height),
            new Vector2(stepx, stepy), countedTime);
    }

    internal void AddUnit(AutomaticGrouping unit, bool isLeader) {
        // if there arent any free leaders, make it lead itself.
        if (isLeader) {
            leaders.Add(unit);
            unit.formation = CreateFormation(unit.timeOutOfRange);
        } else {
            if (!TryAssignToFormation(unit))
                unit.isLeader = isLeader = true;
        }
        groupedUnits.Add(unit);
    }

    private bool TryAssignToFormation(AutomaticGrouping unit) {
        // todo: refactor: have leaders saved by alliances
        List<AutomaticGrouping> leads = leaders;
        List<Alliance> alliedLeaders = UnitsInScene.GetAlliedShips(unit.bomber_v2.info.flag, leaders);
        //List<bool> alliedLeaders = UnitsInScene.GetAlliedLeaderShips(unit.bomber_v2.info.flag, leaders);
        SceneSearching.GetDistanceSorted<Alliance>(unit.transform.position, ref alliedLeaders);
        for (int i = 0; i < alliedLeaders.Count; i++) {
            if (leads[i] == null) continue;
            leads[i].formation.UpdateSize(leads[i].timeOutOfRange);
            if (!leads[i].formation.isFull) {
                leads[i].formation.AddChild(unit);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="isLeader"></param>
    /// <param name="removeChildren">prevent subleaders to break formation completly. sub leaders dont work i think</param>
    internal void RemoveUnit(AutomaticGrouping unit, bool isLeader, bool removeChildren = true) {
        if (isLeader) leaders.Remove(unit);
        groupedUnits.Remove(unit);
        if (removeChildren && unit.formation != null)
        for (int i = 0; i < unit.formation.assignedChildren.Count; i++) {
            AutomaticGrouping subunit = unit.formation.assignedChildren[i];
            RemoveUnit(subunit, false, false);// every man for himself...
        }
    }
}
