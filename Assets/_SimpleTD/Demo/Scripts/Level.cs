using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Level : MonoBehaviour {

    internal bool spawnComplete;

    EnemyCounter spawnedUnits;
    public LevelEnd levelEnd;

    /// <summary>
    /// edit levels here via inspector
    /// </summary>
    public Wave[] waves;

    void Start() {
        spawnedUnits = new EnemyCounter();
        StartCoroutine(SpawnLevel());
    }

    public IEnumerator SpawnLevel() {
        OnLevelBeginSpawn();
        for (int i = 0; i < waves.Length; i++) {
            Wave wave = waves[i];
            yield return new WaitForSeconds(wave.delayBeforeWave);
            for (int g = 0; g < wave.spawnGroups.Count; g++) {
                WaveGroup spawnGroup = wave.spawnGroups[g];
                if (g > 0) {
                    yield return new WaitForSeconds(wave.GetDelay(spawnGroup));
                }
                for (int unitCount = 0; unitCount < spawnGroup.amount; unitCount++) {
                    Spawn(spawnGroup.unitPref, spawnGroup, wave);
                    yield return new WaitForSeconds(spawnGroup.spawnRate);
                }
            }
        }
        OnLevelSpawnedAll();
    }

    private void Spawn(Transform prefab, WaveGroup spawnGroup, Wave wave) {
        Transform unit = Instantiate(prefab) as Transform;
        Waypoint wp = wave.GetSpawnPoint(spawnGroup).GetComponent<Waypoint>();
        unit.GetComponent<Unit>().Init(wp);

        spawnedUnits.Add(unit);
    }

    private void OnLevelBeginSpawn() {
        //throw new System.NotImplementedException();
    }

    private void OnLevelSpawnedAll() {
        //throw new System.NotImplementedException();
        spawnComplete = true;

        StartCoroutine(WaitUntilNoUnitsLeft());
    }

    private IEnumerator WaitUntilNoUnitsLeft()
    {
        while (true)
        {
            if (spawnedUnits.NoneLeft())
            {
                break;
            }
            yield return null;
        }
        BuiltStructures.DisableInteractions();
        levelEnd.FinishedLevelByRunningOutOfEnemies();
    }
}
public static class BuiltStructures
{

    public static List<Transform> turrets = new List<Transform>();

    public static void RegisterTurret(Transform turret)
    {
        turrets.Add(turret);
    }

    public static void DisableInteractions()
    {
        // set "used" on all turrets[turret build control] to false.

        throw new NotImplementedException();
    }
}
public class EnemyCounter
{
    List<Transform> unitsList = new List<Transform>();

    internal void Add(Transform unit)
    {
        unitsList.Add(unit);
    }

    /// <summary>
    /// Units get destroyed either by towers or by passing end checkpoint
    /// </summary>
    /// <returns></returns>
    internal bool NoneLeft()
    {
        unitsList.RemoveAll(i=>i==null);
        return unitsList.Count == 0;
    }
}

[System.Serializable]
public class Wave {

    public string name;
    public float delayBeforeWave;

    public List<WaveGroup> spawnGroups = new List<WaveGroup>();


    public GroupDelays groupsDelays;

    public SpawnPoints spawnPointsPerGroup;

    // add check if this group is in list of custom groups
    internal Transform GetSpawnPoint(WaveGroup spawnGroup) {
        return spawnPointsPerGroup.defaultSpawnPoint;
    }

    // add check if this group is in list of custom groups
    internal float GetDelay(WaveGroup spawnGroup) {
        return groupsDelays.defaultDelayBetweenGroups;
    }
}
[System.Serializable]
public class SpawnPoints {
    public Transform defaultSpawnPoint;
    public CustomSpawnPoint[] customSpawnPoints;
}

[System.Serializable]
public class GroupDelays {
    /// <summary>
    /// int: index of group for custom delay, float: delay
    /// </summary>
    public float defaultDelayBetweenGroups;
    public CustomBetweenGroupDelay[] delaysBetweenGroups;
}

[System.Serializable]
public class WaveGroup {
    public Transform unitPref;
    public int amount;

    public float spawnRate;
    public float timeBeforeFirstUnit;

    
}

[System.Serializable]
public class CustomBetweenGroupDelay {
    public int targetGroup;
    public float customGroupDelay;
}

[System.Serializable]
public class CustomSpawnPoint {
    public int targetGroup;
    public Transform customSpawnPoint;
}



class Prop : UnityEditor.PropertyDrawer {

}
