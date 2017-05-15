using UnityEngine;
using System.Collections;

public class Asteroid : MonoBehaviour {

    public Vector3
        small = new Vector3(0.1f, 0.1f),
        medium = new Vector3(1f, 1f),
        big = new Vector3(5f, 5f);

    public float smallWeight = 0.3f,
        mediumWeight = 0.5f,
        bigWeight = 0.2f;
    string size = "medium"; // small, medium, big

    // Use this for initialization
    void Start() {
        Vector3 randSize = new Vector3();
        float randFactor = Random.Range(0, 1);
        float size
            = randFactor <= smallWeight ? smallWeight
            : randFactor <= mediumWeight ? mediumWeight
            : bigWeight;

        randSize = (small * smallWeight + medium * mediumWeight
            + big * bigWeight) * size * (bigWeight-smallWeight);
    }

	
	// Update is called once per frame
	void Update () {
        transform.Rotate(new Vector3(0, 0, 10));
	}
}
