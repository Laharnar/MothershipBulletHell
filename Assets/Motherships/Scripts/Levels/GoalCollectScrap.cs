using UnityEngine;
using System.Collections;

public class GoalCollectScrap : GoalsDisplay{

    ScrapManager sc;


    public int amountOfScrapNeeded = 300;


    void Start() {
        sc = GameObject.FindObjectOfType<ScrapManager>();
        UpdateDisplay();
    }

    void Update() {
        if (!goalReached && sc.CollectedScrap >= amountOfScrapNeeded ) {
            goalReached = true;
            UpdateDisplay();
        } else {
            if (!goalReached)
                UpdateDisplay();
        }
    }

    public override void UpdateDisplay() {

        display.text = prefix + Mathf.Clamp(sc.CollectedScrap, 0, amountOfScrapNeeded) + separator + amountOfScrapNeeded + suffix;            

        OnGoalReached();
    }


}
