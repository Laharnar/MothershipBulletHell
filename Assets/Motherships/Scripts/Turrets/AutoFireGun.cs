using UnityEngine;
using System.Collections;

public class AutoFireGun : GunBase {

    //public bool fire = false;

    //public float fireRate;
    //public Transform bullet;
    //public Transform spawnPoint;

	// Use this for initialization
	

    public void AutoFireOn(bool active) {
        fire = active;
    }
}
