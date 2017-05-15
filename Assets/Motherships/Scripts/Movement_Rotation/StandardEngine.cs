using UnityEngine;
using System.Collections;

[@RequireComponent(typeof(UpdateQueue))]
public class StandardEngine : Engine {

    public bool startAtFullPower = false;
    protected float power = 0.0f;//current power

    

    public Rigidbody2D rigidbody2d;

    internal bool restrictMovement = false; // optionaly move slower at any time, ussualy on destroyed


    public float restrictMultiplier = 0.75f;


    Vector3 curVelocity;



    protected override void OnStart() {
        base.OnStart();

        if (startAtFullPower) {
            power = 1;
        }

        StartCoroutine(CustomPhysicsPosKeeping());
    }

    private IEnumerator CustomPhysicsPosKeeping() {
        while (true) {
            Vector3 lastPosition = transform.position;
            yield return new WaitForFixedUpdate();
            Vector3 newPosition = transform.position;
            OnMove(lastPosition, newPosition);
        }
    }

    protected override void EnginePositionUpdate() {
        devRunning = false;
        if (suspended) {
            return;
        }
        AccelerationEngineUpdate();
    }

    protected virtual void AccelerationEngineUpdate() {
        Accelerate(enginePower);
    }

    protected virtual void Accelerate(float enginePower) {
        devRunning = true;
        if (restrictMovement) {
            enginePower = enginePower * restrictMultiplier;// reduce max engine speed by 25%
        }
        float speed = rigidbody2d.velocity.sqrMagnitude;

        float power = this.power;
        this.power = enginePower * Time.deltaTime;//lerp it up to 1

        rigidbody2d.AddRelativeForce(Vector2.up * power);// more realistic flaying, bettwr for rockets

        if (speed > enginePower) {
            rigidbody2d.velocity = Vector3.Normalize(rigidbody2d.velocity) * Mathf.Sqrt(enginePower);
        }

        curVelocity = rigidbody2d.velocity;
    }

    public void Run(bool run) {
        restrictMovement = !run;
    }

}
