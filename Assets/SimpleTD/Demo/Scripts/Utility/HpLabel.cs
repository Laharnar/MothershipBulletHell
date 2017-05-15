using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HpLabel : MonoBehaviour {

    public Text label;
    public Text numLabel;

    public HpControl hpToDisplay;

    // Use this for initialization
    void Start () {
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
            numLabel.text = hpToDisplay.hp.ToString();
    }
}
