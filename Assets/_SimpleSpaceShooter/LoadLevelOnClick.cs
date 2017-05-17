using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class LoadLevelOnClick : MonoBehaviour {

    public string levelName;

	// Use this for initialization
	void OnMouseUp () {
        LoadLevelByName();
	}

    public void LoadLevelByName() {
        SceneManager.LoadScene(levelName);
    }

    public void LoadLevelByName(string levelName) {
        SceneManager.LoadScene(levelName);
    }
}
