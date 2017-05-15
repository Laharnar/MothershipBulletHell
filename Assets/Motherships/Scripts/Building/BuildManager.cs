using UnityEngine;
using System.Collections;

public class BuildManager : MonoBehaviour {
    ScrapManager sm;

    void Start() {
        sm = GameObject.FindObjectOfType<ScrapManager>();
    }

	// Update is called once per frame
	public void Build (Structure prefabConnection) {

        if (sm.collectedDrop < 300) {
            return;
        }

        Transform t = Instantiate(prefabConnection.prefab) as Transform;
        t.GetComponent<LaserGun>().Init(prefabConnection);

        sm.UseScrap(300);

        GameObject.FindObjectOfType<GoalBuildStructure>().OnBuildSomething(t.GetComponent<LaserGun>());
	}
}
