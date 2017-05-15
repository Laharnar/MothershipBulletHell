using UnityEngine;
using System.Collections;

/// <summary>
/// handles all clicking logic around towers
/// </summary>
public class TowerBuildManager : MonoBehaviour {

    public Transform[] towerPrefs;
    public Transform removePos;


    void Start() {

    }

	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            RaycastHit2D selected = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            BuildLocation bl = selected.transform.GetComponent<BuildLocation>();
            if (bl) {
                print("location up");
            }

            Tower tw = selected.transform.GetComponent<Tower>();
            if (tw) {
                print("towerup");
            }
        }
	}
}
