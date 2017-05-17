using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GoldLabel : MonoBehaviour {

    public Text label;
    public Text numLabel;

    public GoldCollected goldToDisplay;

    void Start()
    {
        if (!numLabel)
        {
            Debug.LogWarning("Number label isnt assigned");
        }

    }

    void Update()
    {
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        if (numLabel)
            numLabel.text = goldToDisplay.gold.ToString();
    }
}
