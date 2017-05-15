using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Displays and keeps current scrap in sync with scrap manager.
/// 
/// Put it on main UI object. This is just a temporary buggy solution for 1 upgrade
/// </summary>
/// <seealso cref="ScrapManager.cs"/>
public class UpgradeText : MonoBehaviour {

    public Text textControl;

    int requiredScrap = 300;
    int currentScrap { get { return m.CollectedScrap; } }

    ScrapManager m;

    public bool open {
        set {
            textControl.transform.parent.gameObject.SetActive(value);
        }
    }

	// Use this for initialization
	void Start () {
        m = GameObject.FindObjectOfType<ScrapManager>();
        this.InspectorNullComponentWarning(m, "Missing object in scene = ScrapManager");
	}

    public void UpdateDisplay() {
        if (textControl.gameObject.activeInHierarchy) {
            textControl.text = "(" + currentScrap + "/" + requiredScrap + ")";
            
        }
    }
}
