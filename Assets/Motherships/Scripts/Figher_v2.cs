using UnityEngine;
using System.Collections;

/// <summary>
/// Refuels at capital ship every time it reaches low fuel.
/// 
/// Immune to bombers(avoids them).
/// 
/// Strong forward fire plus automatic rotating light gun that fires at closest target
/// 
/// Attacks closes enemies in assigned zone until they are destroyed
/// </summary>
public class Figher_v2 : MonoBehaviour {

    public ShipInfo info;
    public RefuelControl enginesMonitor;
    public float scanRange = 50;

    Transform activeTarget;

    public AnimationCurve rotationCurve;
    public float mobility = 10;
    public float closestDistance = 8;
    public float evoidToDistance = 50;
    //public GunControl gunControl;

    public Transform rightPoint;
    public Transform leftPoint;

    public Spaceship refuelStation;
    public SimpleUnit source;

    //.....................
    public SimpleGun forwardGun;
    public SimpleGun rotatingGun;


    // Use this for initialization
    void Start() {
        StartCoroutine(AttackPattern());
    }

    private IEnumerator AttackPattern() {
        while (true) {
            // scan pattern
            if (!activeTarget) {
                Collider2D possibleTarget = transform.Collision2DScan(scanRange, info.flag.alliance);
                if (possibleTarget != null) {
                    // attack pattern
                    yield return Attack(possibleTarget.transform);
                }
            } else {
                yield return Attack(activeTarget);
            }
            if (enginesMonitor.needToRefuel) {
                yield return Refuel();
            }
            yield return null;
        }
    }

    private IEnumerator Refuel() {
        if (!refuelStation) Debug.Log("Error");

        yield return SceneSearching.TravelInPath(transform, new Vector3[1] { refuelStation.transform.position }, refuelStation.refuelRange);
        //while (Vector3.Distance(refuelStation.transform.position, transform.position) > refuelStation.refuelRange) {
            //TravelTo(refuelStation.transform.position);
          //  yield return null;
        //}
        enginesMonitor.RefillFuel();
    }

    private IEnumerator Attack(Transform possibleTarget) {
        activeTarget = possibleTarget;
        while (activeTarget != null) {
            if (enginesMonitor.needToRefuel) {
                break;
            }

            //enginesMonitor.monitoredEngine.Run(true);
            //source.ExecuteMove(SimpleUnit.Moves.forward);
            EnableForwardGuns();

            SetRotatingGunsTarget(activeTarget);
            if (activeTarget)
            yield return StartCoroutine(SceneSearching.TravelInPath(transform, new Vector3[1] { activeTarget.position }, closestDistance));
            //if (Vector3.Distance(activeTarget.position, transform.position) > closestDistance) {
                //TravelTo(activeTarget.position);

            //} else {
            DisableForwardGuns();
            yield return StartCoroutine(Avoid());
            //}
            yield return null;
        }
        DisableForwardGuns();
        //enginesMonitor.monitoredEngine.Run(false);
        source.Idle(instantlyResetRot: true);
    }

    private void TravelTo(Vector3 targetPosition) {
        //transform.SmoothRotateTowards(targetPosition, mobility, sideEngine);
    }

    private IEnumerator Avoid() {
        int choice = ((int)Time.time) % 2;
        float time = Time.time + 1;
        //rotate away for a while. its still going forward
        while (time > Time.time) {
            if (!activeTarget) {
                break;
            }

            float dist = Vector3.Distance(activeTarget.position, transform.position);
            if (dist > evoidToDistance) {
                break;
            }
            if (choice == 0) {
                SceneSearching.Follow(transform, source, leftPoint, false);// it will never arrive at this point.
            } else {
                SceneSearching.Follow(transform, source, rightPoint, false);// it will never arrive at this point.
            }
            yield return null;
        }
        source.Idle(resetRotation:false);
    }

    private void GoTowards(Vector3 target) {
        //transform.SmoothRotateTowards(target, mobility, sideEngine);
    }

    internal void SetRotatingGunsTarget(Transform activeTarget) {
        rotatingGun.shootingLogic.SetTarget(activeTarget);// also fires at it(enables guns)
    }

    internal void EnableForwardGuns() {
        forwardGun.shootingLogic.Fire();
    }

    internal void DisableForwardGuns() {
        forwardGun.shootingLogic.HoldFire();
    }
}
