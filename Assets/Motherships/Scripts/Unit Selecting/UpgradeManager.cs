using UnityEngine;
using System.Collections;

public class UpgradeManager : MonoBehaviour {

    public UnitSelector selection;

    bool open = false;


    UpgradePlatform[] allPlatforms;

    void Start() {
        allPlatforms = GameObject.FindObjectsOfType<UpgradePlatform>();
    }

	// Update is called once per frame
	void Update () {
        if (selection.selectionChanged) {
            if (selection.areUnitsSelected) {
                // shows buttons for all selected platforms
                open = true;
                UpgradePlatform[] selectedPlatforms = selection.GetUpgradePlatforms;

                for (int i = 0; i < selectedPlatforms.Length; i++) {
                    selectedPlatforms[i].ShowButton(true);
                }
            } else {
                // shows buttons for all selected platforms
                open = false;
                for (int i = 0; i < allPlatforms.Length; i++) {
                    allPlatforms[i].ShowButton(false);
                }
            }
            
        }
	}
}
