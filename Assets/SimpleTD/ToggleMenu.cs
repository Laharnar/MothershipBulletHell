using UnityEngine;
using System.Collections;

public class ToggleMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	public void Toggle (Transform target) {
        target.gameObject.SetActive(!target.gameObject.activeSelf);
	}

}
