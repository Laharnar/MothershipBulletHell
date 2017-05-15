using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct WaveInfo {
    public AttackingWave.EnemyType enemyType;
    public int count;
}

public class AttackingWave : MonoBehaviour {

    public EnemyType[] enemyTypes;

    public struct EnemyType {
        public Transform enemyPrefab;
        public string name;
    }

    int m_currentWaveIndex;
    public List<WaveInfo> m_waves;

    public void AddLevel(string reg_enemy) {
        //\(([A-Z])x([0-9(?=ABC)]*)\)
    }


    public void AddWave(EnemyType enemy, int enemyCount) {
        WaveInfo info = new WaveInfo() { enemyType = enemy, count = enemyCount };
        m_waves.Add(info);
    }

    public void ResetLevel() {
        m_currentWaveIndex = 0;
    }

    public void NextWave() {
        m_currentWaveIndex = Mathf.Min(m_currentWaveIndex + 1, m_waves.Count - 1);
    }

    public WaveInfo CurrentWave {
        get {
            return m_waves[m_currentWaveIndex];
        }
    }

    public int CurrentWaveIndex {
        get {
            return m_currentWaveIndex;
        }
    }
}
