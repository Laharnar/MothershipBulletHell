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
public class Fighter : Spaceship {

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
    void Start () {
        StartCoroutine(AttackPattern());
	}

    private IEnumerator AttackPattern() {
        while (true) {
            // scan pattern
            if (!activeTarget) {
                Collider2D possibleTarget = transform.Collision2DScan(scanRange, alliance);
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
        while(Vector3.Distance(refuelStation.transform.position, transform.position) > refuelStation.refuelRange){
            TravelTo(refuelStation.transform.position);
            yield return null;
        }
        enginesMonitor.RefillFuel();
    }

    private IEnumerator Attack(Transform possibleTarget) {
        activeTarget = possibleTarget;
        while (activeTarget != null) {
            if (enginesMonitor.needToRefuel) {
                break;
            }

            //enginesMonitor.monitoredEngine.Run(true);
            source.ExecuteMove(SimpleUnit.Moves.forward);
            EnableForwardGuns();

            SetRotatingGunsTarget(activeTarget);

            if (Vector3.Distance(activeTarget.position, transform.position) > closestDistance) {
                TravelTo(activeTarget.position);

            } else {
                DisableForwardGuns();
                yield return StartCoroutine(Avoid());
            }
            yield return null;
        }
        DisableForwardGuns();
        //enginesMonitor.monitoredEngine.Run(false);
        source.Idle(instantlyResetRot:true);
    }

    private void TravelTo(Vector3 targetPosition) {
        transform.SmoothRotateTowards(targetPosition, mobility, sideEngine);
    }

    private IEnumerator Avoid() {

        source.Forward();

        int choice = ((int)Time.time) % 2;
        float time = Time.time+1;
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
                GoTowards(leftPoint.position);
            } else {
                GoTowards(rightPoint.position);
            }
            yield return null;
        }
        // just wait until out of range, forward movement is still working
        while (true) {
            if (!activeTarget) {
                break;
            }
            float dist = Vector3.Distance(activeTarget.position, transform.position);
            if (dist > evoidToDistance) {
                break;
            }
            yield return null;
        }
    }

    private void GoTowards(Vector3 target) {
        transform.SmoothRotateTowards(target, mobility, sideEngine);
    }

    internal void SetRotatingGunsTarget(Transform activeTarget) {
        rotatingGun.shootingLogic.SetTarget(activeTarget);//  also fires at it
        //rotatingGun.FocusFire(activeTarget);
        //rotatingGun.fire = true; // gun should start firing automaticaly
    }

    internal void EnableForwardGuns() {
        forwardGun.shootingLogic.Fire();
    }

    internal void DisableForwardGuns() {
        forwardGun.shootingLogic.HoldFire();
    }

    /*private Collider2D Scan() {
        Collider2D[] scanned = Physics2D.OverlapCircleAll(transform.position, scanRange, 1 << LayerMask.NameToLayer("Scan"));
        Collider2D enemy = null;
        for (int i = 0; i < scanned.Length; i++) {
            Spaceship sp = scanned[i].transform.root.GetComponent<Spaceship>();
            if (sp && sp.alliance != alliance) {
                enemy = scanned[i];
                break;
            }
        }
        Debug.DrawRay(transform.position - (Vector3.up + Vector3.right) * scanRange, (Vector3.up + Vector3.right) * 2 * scanRange);
        Debug.DrawRay(transform.position - (Vector3.up + Vector3.left) * scanRange, (Vector3.up + Vector3.left) * 2 * scanRange);
        return enemy;
    }*/

    /*void Steering(Vector3 targetPosition) {

        if (sideEngine.restrictRotation) {
            mobility = mobility * 0.5f;// increase time it takes to make the turn
        }

        Quaternion newRotation = Quaternion.LookRotation(transform.position - targetPosition, Vector3.forward);
        newRotation.x = 0.0f;
        newRotation.y = 0.0f;
#if realisticRotation
        //transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, rotationCurve.Evaluate(Time.deltaTime * mobility));
#endif
        
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * mobility);
        Debug.DrawLine(transform.position, targetPosition);
    }*/

}
