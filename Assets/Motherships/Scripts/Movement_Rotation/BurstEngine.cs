using UnityEngine;
using System.Collections;

/// <summary>
/// Provides an engine that accelerates until fuel runs out.
/// </summary>
public class BurstEngine : Engine {

    public float fuelStorage = 1;

    float fuel;
    
    /// <summary>
    /// how much fuel per second it uses
    /// </summary>
    public float releaseRate = 1;

    /// <summary>
    /// Release rate effectiveness, decreses in atmosphere.
    /// range 0-1
    /// </summary>
    public float releaseEffectiveness = 1;
    float engineEffectivness;

    float currentForce = 0;
    
    //public AnimationCurve accelerationCurve;

    //float lerp = 0;

    //void ApplyAcceleration() {
        //lerp = Mathf.Lerp(transform.position, target.position, accelerationCurve.Evaluate(Time.time));
    //}

    void Start() {
        fuel = fuelStorage;

        StartCoroutine(StartEngines());
    }

    private IEnumerator StartEngines() {
        engineEffectivness = 0;
        while (engineEffectivness < releaseEffectiveness && fuel > 0) {
            engineEffectivness = Mathf.Lerp(engineEffectivness, 1, Time.deltaTime);
            yield return null;
        }
    }
	
	// Update is called once per frame
	void Update () {

        float speed = currentForce;

        if (fuel > 0f) {
            // calculate fuel used in next step. generate force from it and try to reach new force,
            float fuelUsed = Mathf.Clamp(fuel - (fuel - (releaseRate * Time.deltaTime)), 0, fuel);

            float generatedForce = fuelUsed * engineEffectivness;

            currentForce = Mathf.Lerp(currentForce, currentForce + generatedForce, Time.deltaTime);

            // reduce fuel. its clamped to 0 above at calculation
            fuel -= fuelUsed;
        }


        MoveForward(speed);
	}

    private void MoveForward(float speed) {
        transform.Translate(Vector2.up * speed, Space.Self);

        Debug.DrawRay(transform.position - Vector3.up , Vector3.up *2, Color.red, 0.01f);
        Debug.DrawRay(transform.position - Vector3.right , Vector3.right * 2, Color.red, 0.01f);
    }
}
