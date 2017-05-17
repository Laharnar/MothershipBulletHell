using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActivateBehaviourUtility : CollisionUtility {

    List<MonoBehaviour> registeredBehaviour = new List<MonoBehaviour>();

    public bool setToEnabled = true;
    public bool setOppositeOnRegister = true;// in awake we have registering
    public bool disableCollisionAfterUse = true;

    public void Register(MonoBehaviour movement) {
        registeredBehaviour.Add(movement);
        if (setOppositeOnRegister)
            movement.enabled = !setToEnabled;
    }

    public override void Activate() {
        base.Activate();

        for (int i = 0; i < registeredBehaviour.Count; i++) {
            if (registeredBehaviour[i] == null) continue;
            registeredBehaviour[i].enabled = setToEnabled;

            if (registeredBehaviour[i].GetType() == typeof(Shoot))
                ((Shoot)registeredBehaviour[i]).OnCustomEnabled();

            else if (registeredBehaviour[i].GetType() == typeof(SimplestMovement))
                ((SimplestMovement)registeredBehaviour[i]).OnCustomEnabled();
        }

        if (disableCollisionAfterUse) {
            gameObject.SetActive(false);
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(transform.position, new Vector3(27, 1));
    }

}
