using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// can syncronize with others
/// <summary>
/// Fires rockets :)
/// 
/// By behaviour, it fires all shots at biggest (aproximated) groups of enemies, and then either reloads one or all shots
/// </summary>
public class RocketLauncher : AimingGunBase {

    public SpawnerBase rocketTubes;

    //CentralizedTargetingWithAproximation
    //public ScanRangeTracking tracking; // tracks all targets in scan
    //public MaxGroupsFilter targetChoice; // selects targets that are grouped

    public bool ready = true;
    public float mininumDelayBetweenShots;
    public int maxReloadNumber = 1;// how many shots will it reload before shooting again, IF its attacking something
    public bool paralelLoading = false; // true: tubes can be all loaded at same time -> instant reload.  false: tubes load 1 by 1, allowing starting burst then balanced over time damage of 1 rocket by 1

    bool fullClipReload = false;

    protected override void OnStart() {
        base.OnStart();
    }

    protected override IEnumerator Fire() {

        /*float[] timers = new float[rocketTubes.spawnPoints.Length];
        foreach (var timer in timers) {
            timer = Time.time;
        }*/

        Reaim();

        rocketTubes.fired = new bool[rocketTubes.spawnPoints.Length];

        while (true) {

            if (Reaim()){
                fire = true;
        }else fire = false;

            if (fire) {
                yield return FireAllShots();
                // dont put reloading here, since fire variable can change while shooting
            }

            int reloadsLeft = 0;
            if (fire) {
                // partial reload
                reloadsLeft = Mathf.Clamp(maxReloadNumber, 0, rocketTubes.clipSize.Length);
                for (int i = 0; i < rocketTubes.clipSize.Length; i++) {
                    if (!rocketTubes.fired[i]) {
                        reloadsLeft--;
                    }
                }

                yield return Reload(reloadsLeft);
            } else {
                // full reload
                reloadsLeft = rocketTubes.clipSize.Length;
                for (int i = 0; i < rocketTubes.clipSize.Length; i++) {
                    if (!rocketTubes.fired[i]) {
                        reloadsLeft--;
                    }
                }
                fullClipReload = true;
            }

            yield return Reload(reloadsLeft);
            if (fullClipReload) {
                fullClipReload = false;
            }
            yield return null;
        }
    }

    private bool Reaim() {
        //if (lastUnitCount != allUnits.allShips.Count) {

        float explosionRadius = projectile.GetComponent<Rocket>().radius;
        Transform targetChosen = UnitsInScene.GetApproximationOfGroupCount(this,explosionRadius * 2f);
        if (targetChosen) {
            AimAtTarget(targetChosen);// *2 to get less calculations
            activeTarget = targetChosen;

        }
        return activeTarget != null;

        //}
    }

    /// <summary>
    /// Reloads up to maximum amount of clips.
    /// 
    /// Assumes every tube has only one shot.
    /// Tubes are chosen randomly from empty ones
    /// </summary>
    /// <param name="reloadsLeft"></param>
    /// <returns></returns>
    private IEnumerator Reload(int reloadsLeft) {
        fire = false;
        List<int> emptyTubes = new List<int>();
        for (int i = 0; i < rocketTubes.clipSize.Length; i++)
		{
            if (!rocketTubes.fired[i]) {
                emptyTubes.Add(i);
            }
		}
        while (reloadsLeft > 0) {
            yield return new WaitForSeconds(rocketTubes.clipReloadTime);
            int reloadtube = Random.Range(0, rocketTubes.clipSize.Length);
            rocketTubes.fired[reloadtube] = false;
            emptyTubes.Remove(reloadtube);

            reloadsLeft -= 1;

            // starting fire in middle of reloading will immediatly begin firing(useful to stop full clip reload if new enemy enters)
            // comment these 2 lines so full clip reload cant be stopped
            if (fire && fullClipReload) {
                break;
            }
        }

        fire = true;
    }

    private IEnumerator FireAllShots() {
        for (int i = 0; i < rocketTubes.clipSize.Length; i++) {
            if (!fire) {
                break;
            }
            int clipsLeft = rocketTubes.clipSize[i];
            if (clipsLeft > 0) {
                FireBullet(rocketTubes.spawnPoints[i]);
                rocketTubes.fired[i] = true;
                
                yield return ResumeFire(mininumDelayBetweenShots);
                Reaim();
            }
        }
    }

}
