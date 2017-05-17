using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// one per scene!
public class DroneExplosionList : MonoBehaviour {

    List<DroneStack> droneStack = new List<DroneStack>();
    public List<Drone> registeredDrones = new List<Drone>();
    public float chainDestructionSpeed = 0.3f;

    public class DroneStack
    {
        public Stack<Drone> drones = new Stack<Drone>();
    }

	// Use this for initialization
	void Start () {
        StartCoroutine(ChainDestroyDrones());
	}

    private IEnumerator ChainDestroyDrones()
    {
        // every x seconds destroy 1 stack of drones
        while (true)
        {
            // empty all stacks, 1 per wait for x seconds
            if (droneStack.Count > 0)
            {
                // empty all drones in 1 stack
                while (droneStack[0].drones.Count > 0)
                {
                    Drone dr = droneStack[0].drones.Pop();
                    dr.GetUnitsAroundAndRegisterThem(1);
                    dr.gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
                    if ( dr!= null)
                        Destroy(dr.gameObject);
                }
                droneStack.RemoveAt(0);
            }
            yield return new WaitForSeconds(chainDestructionSpeed);
        }
    }

    // true: was accepted
    // false: wasnt
    public bool RegisterDrone(int stackSchedule, Drone drone) {
        // register units for filtering duplicates and null values
        if (drone == null || registeredDrones.Contains(drone) || registeredDrones.Count > 50)
        {
            return false;
        }
        registeredDrones.Add(drone);

        // register drone on empty or existing stack
        while (droneStack.Count <= stackSchedule)
        {
            droneStack.Add(new DroneStack());
        }
        droneStack[stackSchedule].drones.Push(drone);
        return true;
	}
}
