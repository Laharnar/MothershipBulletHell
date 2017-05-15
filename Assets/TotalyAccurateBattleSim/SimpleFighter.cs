using UnityEngine;
using System.Collections;
using System;

public class SimpleFighter : MonoBehaviour {

    SimpleEngineUnit engines;
    SimpleLaserControl lasers;
    Alliance flag;

    // Use this for initialization
    void Start () {
        engines = GetComponent<SimpleEngineUnit>();
        flag = GetComponent<Alliance>();
        lasers = GetComponent<SimpleLaserControl>();
        lasers.onFire += OnFire;

        StartCoroutine(SearchAndDestroy());
	}

    private IEnumerator SearchAndDestroy()
    {
        while (true)
        {

            yield return StartCoroutine(SceneSearching.FollowTarget(transform, engines.followAi.getSource, engines.followAi.ChoseClosestTarget(transform, flag.alliance), false));
        }
    }

    void Update()
    {
        SetLasers();

    }

    void OnFire(Bullet bullet)
    {
        bullet.InitBullet(flag.alliance);
    }

    /// <summary>
    /// Set laser states dpending on which engine is turned on.
    /// </summary>
    private void SetLasers()
    {
        // none
        if (!engines.engines["acceleratingForward"] 
            && !engines.engines["goingLeft"] 
            && !engines.engines["goingRight"])
            lasers.laserState = SimpleLaserControl.LaserState.Laser;

        // l/r only
        else if (!engines.engines["goingForward"]
            && (engines.engines["goingLeft"] 
             || engines.engines["goingRight"]))
            lasers.laserState = SimpleLaserControl.LaserState.Constant;

        // main only
        if (engines.engines["acceleratingForward"]
            && !engines.engines["acceleratingLeft"]
            && !engines.engines["acceleratingRight"])
            lasers.laserState = SimpleLaserControl.LaserState.Bursts;

        // main + l/r
        if (engines.engines["acceleratingForward"]
            && (engines.engines["acceleratingLeft"]
             || !engines.engines["acceleratingRight"]))
            lasers.laserState = SimpleLaserControl.LaserState.SlowFire;
    }
}
