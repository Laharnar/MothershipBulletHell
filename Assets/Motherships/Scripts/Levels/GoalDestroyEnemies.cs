using UnityEngine;
using System.Collections;

public class GoalDestroyEnemies : GoalsDisplay {

    EnemyManager em;
    int startingEnemyCount;
    
    void Start() {
        em = GameObject.FindObjectOfType<EnemyManager>();
        startingEnemyCount = em.MainEnemyCount;
    }

    void Update() {
        UpdateDisplay();
    }

    public override void UpdateDisplay() {

        if (em.MainEnemyCount == 0) {
            goalReached = true;
        }

        display.text = prefix + (startingEnemyCount - em.MainEnemyCount) + separator + startingEnemyCount + suffix;

        OnGoalReached();
    }
}
