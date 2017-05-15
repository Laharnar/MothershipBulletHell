using UnityEngine;
using System.Collections;

[System.Obsolete("Dont use this script, use RepairRegen.cs")]
public class Regen : MonoBehaviour {

    public Spaceship target;

    public float rate = 0.1f;
    public float amount = 0.5f;

    void Start() {
        StartCoroutine(RepairLoop());
    }

    public IEnumerator RepairLoop() {
        while (target) {
            yield return new WaitForSeconds(rate);
            target.Repair(amount);
        }
        Debug.Log("Done, no target to repair.");
    }
}
