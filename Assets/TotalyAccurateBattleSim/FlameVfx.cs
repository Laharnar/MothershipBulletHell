using UnityEngine;
using System.Collections;
using System;

public class FlameVfx : MonoBehaviour {

    Vector3 origPos;
    Vector3 origScl;

    public bool useEfx = true;
    public bool setPosByScale = true;
    public Vector3 dir = Vector3.up;
    public bool vertical = true;
    public bool burning = true;
    bool burnAtMax = false;

    float sizeModifier = 1; // in case you want flame to be smaller for some reason

    // Use this for initialization
    void Start () {
        origPos = transform.localPosition;
        origScl = transform.localScale;
	}
	
    public void Burn()
    {
        burning = true;
        useEfx = true;
    }

    public void StopBurn()
    {
        burning = false;
        useEfx = true;
    }

    public void Burn(bool burn)
    {
        burning = burn;
        useEfx = true;
    }

    internal void Burn(bool burn, float modifier)
    {
        if (modifier == 0) Debug.LogWarning("Modifier should be greater than 0. Turn off effect if you dont want it.");

        sizeModifier = modifier;

        Burn(burn);
    }

    // Update is called once per frame
    void Update () {
        if (!useEfx)
        {
            return;
        }
        if (burning)
        {
            
            // start flame up
            if (!burnAtMax) 
            {

                Vector3 diff = transform.localScale;
                Vector3 reverseScalingCalcululations = new Vector3(
                        transform.localScale.x + (origScl.x*sizeModifier * 0.25f),
                        transform.localScale.y + (origScl.y*sizeModifier * 0.25f),
                        transform.localScale.z + (origScl.z*sizeModifier * 0.25f)
                    );


                transform.localScale = new Vector3(
                        reverseScalingCalcululations.x,
                        reverseScalingCalcululations.y,
                        reverseScalingCalcululations.z
                    );

                Vector3 positionCalculations = new Vector3(
                    dir.x * (diff.x - transform.localScale.x) / 2,
                    dir.y * (diff.y - transform.localScale.y) / 2,
                    dir.z * (diff.z - transform.localScale.z) / 2
                );

                transform.localPosition += new Vector3(
                        positionCalculations.x,
                        positionCalculations.y,
                        positionCalculations.z
                    );

                if (transform.localScale.x >= origScl.x && transform.localScale.y >= origScl.y
                    && transform.localScale.z >= origScl.z)
                {
                    burnAtMax = true;
                }
            }
            else BurnEfx();
        }
        else // reduce flame to 0
        {
            Vector3 diff = transform.localScale;
            Vector3 reverseScalingCalcululations = new Vector3(
                    transform.localScale.x - (transform.localScale.x * 0.40f),
                    transform.localScale.y - (transform.localScale.y * 0.40f),
                    transform.localScale.z - (transform.localScale.z * 0.40f)
                );
            

            transform.localScale = new Vector3(
                    reverseScalingCalcululations.x,
                    reverseScalingCalcululations.y,
                    reverseScalingCalcululations.z
                );

            Vector3 positionCalculations = new Vector3(
                dir.x * (diff.x - transform.localScale.x) / 2,
                dir.y * (diff.y - transform.localScale.y) / 2,
                dir.z * (diff.z - transform.localScale.z) / 2
            );

            transform.position += new Vector3(
                    positionCalculations.x,
                    positionCalculations.y,
                    positionCalculations.z
                );
            burnAtMax = false;
        }
	}


    private void BurnEfx()
    {
        Vector3 diff = transform.localScale;
        Vector3 scalingCalculation = new Vector3(
                (0.85f * origScl.x * sizeModifier + 0.15f * transform.localScale.x),
                (0.85f * origScl.y * sizeModifier + 0.15f * transform.localScale.y),
                (0.85f * origScl.z * sizeModifier + 0.15f * transform.localScale.z)
            );
        Vector3 timeCalculations = new Vector3(
                origScl.x * sizeModifier -0.15f + Mathf.PingPong(Time.time,  0.2f * sizeModifier),
                origScl.y * sizeModifier -0.15f + Mathf.PingPong(Time.time,  0.2f * sizeModifier),
                origScl.z * sizeModifier - 0.15f + Mathf.PingPong(Time.time, 0.2f * sizeModifier)
            );
        Vector3 reducedTimeCalculations = new Vector3(
                origScl.x + Mathf.PingPong(Time.time, 0.05f * sizeModifier),
                origScl.y + Mathf.PingPong(Time.time, 0.05f * sizeModifier),
                origScl.z + Mathf.PingPong(Time.time, 0.05f * sizeModifier)
            );

        if (vertical)
            transform.localScale = new Vector3(reducedTimeCalculations.x, timeCalculations.y, scalingCalculation.z);
        else transform.localScale = new Vector3(timeCalculations.x, reducedTimeCalculations.y, scalingCalculation.z);

        Vector3 positionCalculations = new Vector3(
               dir.x * (diff.x - transform.localScale.x) / 2,
               dir.y * (diff.y - transform.localScale.y) / 2,
               dir.z * (diff.z - transform.localScale.z) / 2
           );

        if (setPosByScale)
        {
            transform.localPosition += new Vector3(
                positionCalculations.x,
                positionCalculations.y,
                positionCalculations.z
                );
        }
    }
}
