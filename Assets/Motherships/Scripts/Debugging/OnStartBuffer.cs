using UnityEngine;
using System.Collections;

public class OnStartBuffer : MonoBehaviour {

    public bool startupWarnings = true;

	// Use this for initialization
	void Start () {
        if (startupWarnings) {
            StartCoroutine(WaitEndFrameAndFireLog());
        }
	}

    private IEnumerator WaitEndFrameAndFireLog() {
        yield return new WaitForEndOfFrame();
        UnityConsole.ReleaseBuffer();
    }
}
