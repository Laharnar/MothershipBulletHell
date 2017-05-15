using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RedirectionMagnet : MonoBehaviour {

    public CentralizedGunTargetTracking effectRange;

    List<Spaceship> ships = new List<Spaceship>();

    public Vector3 direction;

    string alliance = "player";

	// Use this for initialization
	void Start () {
        effectRange.RegisterTargetTracking(OnExit, null, OnEntrance);
	}

    void OnEntrance(Transform target) {
        var s = target.root.GetComponent<Spaceship>();
        if (s && s.alliance != alliance) {
            ships.Add(s);//.ForceTo(transform.position);
        }
    }

    void OnExit(Transform target) {
        var s = target.root.GetComponent<Spaceship>();
        if (s && s.alliance != alliance) {
            ships.Remove(s);//.ClearForce(transform.position);
        }
    }

	// Update is called once per frame
	void LateUpdate () {
        for (int i = 0; i < ships.Count; i++) {
            //ships[i].rigidbody2d.AddForce(direction, ForceMode2D.Impulse);
            ships[i].transform.Translate(Vector3.Lerp(ships[i].transform.position, transform.position, 1000));
        }
	}
}
