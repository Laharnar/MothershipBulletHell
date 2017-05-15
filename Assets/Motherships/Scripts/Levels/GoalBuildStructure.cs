using UnityEngine;
using System.Collections;
using System;

public class GoalBuildStructure : GoalsDisplay {

    Type target = typeof(LaserGun);



    void Start() {
        UpdateDisplay();
    }

    public void OnBuildSomething(LaserGun structure) {
        if (structure.GetType() == target) {
            goalReached = true;
        }
        UpdateDisplay();
    }

    public override void UpdateDisplay() {
        display.text = prefix + (goalReached ? "1": "0") + separator + "1" + suffix;
        
        OnGoalReached();
    }
}