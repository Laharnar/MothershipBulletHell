using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour {

    public Transform target;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 vec = target.position;
        vec.z = transform.position.z;
        transform.position = (vec);
	}
}
