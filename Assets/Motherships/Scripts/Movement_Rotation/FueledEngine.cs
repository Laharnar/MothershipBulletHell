using UnityEngine;
using System.Collections;

public class FueledEngine : StandardEngine {


    public float maxFuel = 200;
    internal float currentFuel;

    public float burnRate = 10;// fuel lasts 10 seconds -> 20 fuel/second
    

    public bool infiniteFuel = false;


    void Awake() {
        currentFuel = maxFuel;

    }

    protected override void AccelerationEngineUpdate() {
        if (!infiniteFuel) {
            if (currentFuel == 0) {
                return;
            }
        }

        base.AccelerationEngineUpdate();

        if (!infiniteFuel) {

            if (!restrictMovement) {
                currentFuel = Mathf.Clamp(currentFuel - (burnRate * Time.deltaTime), 0, maxFuel);
            } else {
                currentFuel = Mathf.Clamp(currentFuel - (burnRate * restrictMultiplier * Time.deltaTime), 0, maxFuel);
            }
        }
    }

    protected override void Accelerate(float enginePower) {
        if (restrictMovement) {
            enginePower = enginePower * restrictMultiplier;// reduce max engine speed by 25%
        }
        float speed = rigidbody2d.velocity.sqrMagnitude;

        float power = this.power;
        this.power = enginePower * Time.deltaTime;//lerp it up to 1

        rigidbody2d.transform.Translate(Vector3.up * power * Time.deltaTime);// more and boring linear flying

        if (speed > enginePower) {
            rigidbody2d.velocity = Vector3.Normalize(rigidbody2d.velocity) * Mathf.Sqrt(enginePower);
        }
    }
}
