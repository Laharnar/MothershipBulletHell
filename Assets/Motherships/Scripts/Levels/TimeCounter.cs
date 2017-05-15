using UnityEngine;
using System.Collections;

public class TimeCounter : MonoBehaviour {

    public TimeDisplay display;
    public bool clockOn;

    void Start() {
        display.SetTime((int)(Time.time / 60), (int)(Time.time % 60));

    }

	// Update is called once per frame
	void Update () {
        if (clockOn) {
            display.SetTime((int)(Time.time / 60), (int)(Time.time % 60));
        }
	}
}
