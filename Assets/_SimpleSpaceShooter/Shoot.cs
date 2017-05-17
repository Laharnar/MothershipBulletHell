using UnityEngine;
using System.Collections;

public class Shoot : SimplestSpawner {

    public bool firstBulletReady = true;
    public float rate = 1;
    float time = 0;

    // for player
    public bool useKey = false;
    public KeyCode key;

    public bool resetTimeOnEnabled = true;

    bool passedFirstFrame = false;

    void Start() {
        if (firstBulletReady)
            time = Time.time;
        else time = Time.time + rate;
    }

	// Update is called once per frame
	protected void Update () {
        passedFirstFrame = true;
        if (Time.time >= time) {
            if (!useKey || Input.GetKey(key)) {
                Spawn();
                time = Time.time + rate;
            }
        }
    }

    internal void OnCustomEnabled() {
        if (resetTimeOnEnabled)
            time = Time.time;
    }
}
