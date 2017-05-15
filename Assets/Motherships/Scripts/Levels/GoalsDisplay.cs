using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class GoalsDisplay : MonoBehaviour {

    public Text display;

    string text;
    public string prefix;
    public string separator = "/";
    public string suffix;
    public bool goalReached { get; protected set; }

    public void Show(bool visible) {
        display.enabled = visible;
    }

    public virtual void UpdateDisplay() {
        if (display.enabled) {
            display.text = prefix+"Todo: set text for displaying "+suffix;

            OnGoalReached();
        }
    }

    protected void OnGoalReached() {
        if (goalReached) {
            display.color = Color.green;
        }
    }
}
