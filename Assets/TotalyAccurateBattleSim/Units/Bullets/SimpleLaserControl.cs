using UnityEngine;
using System.Collections;
using System;

public class SimpleLaserControl : MonoBehaviour {

    public Transform laserPref;
    public Transform bigLaserPref;
    public Transform spawnPoint;
    public Transform bigLaserSpawnPoint;

    public float burstFireRate = 0.3f;
    public float constantFireRate = 0.5f;
    public float slowFireRate = 0.8f;
    public float laserLife = 1;

    public bool fire = false;

    float activeFireRate;

    Transform tempChild;

    public LaserState laserState = LaserState.Idle;
    internal Action<Bullet> onFire;

    public float maxLaserLength = 100;
    public float bigLaserScaleSpeed = 5;

    public enum LaserState
    {
        Idle = 0,
        SlowFire,
        Bursts,
        Constant,
        Laser,
        Undefined
    }


    void Start()
    {
        StartCoroutine(Fire());
    }


    IEnumerator Fire()
    {

        // perp the big laser
        Transform bigLaserInstance = Instantiate(bigLaserPref,
            bigLaserSpawnPoint.position, bigLaserSpawnPoint.rotation, bigLaserSpawnPoint) as Transform;

        Vector3 originalBigLaserScale = bigLaserInstance.localScale;
        bigLaserInstance.position = bigLaserSpawnPoint.position
            + new Vector3(0, originalBigLaserScale.y / 2, 0);

        // fire
        float time = 0;
        //if (true/*needToreloadFirstTime*/)
        time = Time.time + activeFireRate;

        int burstShotCount = 0;
        bool burstBreak = false;
        float burstbreakTime = 0;
        float timeBetweenBursts = 1f;

        while (true)
        {
            if (laserState != LaserState.Bursts 
                || burstBreak && Time.time > burstbreakTime)
            {
                burstBreak = false;
            }

            if (!fire)
            {
                bigLaserInstance.localScale = new Vector2();
                yield return null;
                continue;
            }

            if (laserState == LaserState.Idle)
            {
                bigLaserInstance.localScale = new Vector2();
            }

            if (laserState == LaserState.SlowFire)
            {
                activeFireRate = slowFireRate;
                bigLaserInstance.localScale = new Vector2();
                if (time < Time.time)
                {
                    time = Time.time + activeFireRate;
                    Transform bullet = Instantiate<Transform>(laserPref);
                    bullet.position = spawnPoint.position;
                    bullet.rotation = spawnPoint.rotation;

                    if (onFire != null)
                        onFire(bullet.GetComponent<Bullet>());

                    Destroy(bullet.gameObject, laserLife);
                }
            }
            // burst lasers
            if (laserState == LaserState.Bursts && !burstBreak)
            {
                activeFireRate = burstFireRate;
                bigLaserInstance.localScale = new Vector2();
                if (time < Time.time)
                {
                    burstShotCount = (++burstShotCount) % 4;
                    if (burstShotCount == 0)
                    {
                        burstBreak = true;
                        burstbreakTime = Time.time + timeBetweenBursts;
                    }

                    time = Time.time + activeFireRate;
                    Transform bullet = Instantiate<Transform>(laserPref);
                    bullet.position = spawnPoint.position;
                    bullet.rotation = spawnPoint.rotation;
                    Destroy(bullet.gameObject, laserLife);
                }
            }
            if (laserState == LaserState.Constant)
            {
                activeFireRate = constantFireRate;
                bigLaserInstance.localScale = new Vector2();
                if (time < Time.time)
                {
                    time = Time.time + activeFireRate;
                    Transform bullet = Instantiate<Transform>(laserPref);
                    bullet.position = spawnPoint.position;
                    bullet.rotation = spawnPoint.rotation;
                    Destroy(bullet.gameObject, laserLife);
                }
            }
            // big laser
            if (laserState == LaserState.Laser)
            {
                if (bigLaserInstance.localScale.y <maxLaserLength)
                bigLaserInstance.localScale = new Vector2(originalBigLaserScale.x, bigLaserInstance.localScale.y + bigLaserScaleSpeed*Time.deltaTime);
                bigLaserInstance.localPosition = new Vector2(0, bigLaserInstance.localScale.y / 2);
            }
            yield return null;
        }
    }

}
