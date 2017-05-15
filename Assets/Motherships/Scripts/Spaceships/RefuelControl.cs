using UnityEngine;
using System.Collections;

public class RefuelControl : MonoBehaviour {

    //public FueledEngine monitoredEngine;
    public FuelTank fuelTank;

    internal bool needToRefuel { get; private set; }
    internal bool fuelCriticalyLow { get; private set; }

    public bool devUseRefuelControl = true;


	// Use this for initialization
	void Start () {
        needToRefuel = false;
        fuelCriticalyLow = false;
        StartCoroutine(MonitorFuel());
	}

    private IEnumerator MonitorFuel() {
        needToRefuel = false;
        fuelCriticalyLow = false;
        while (true) {
            if (!devUseRefuelControl)
	        {
		         break;
	        }

            if (fuelTank.currentFuel < 5 / 8) {
                needToRefuel = true;
            } else if (fuelTank.currentFuel < 1 / 4) {
                fuelCriticalyLow = true;
            }
            /*if (monitoredEngine.currentFuel < monitoredEngine.maxFuel * 5 / 8) {
                needToRefuel = true;
            } else if (monitoredEngine.currentFuel < monitoredEngine.maxFuel * 1 / 4) {
                fuelCriticalyLow = true;
            }*/
            yield return null;
        }
        
    }


    internal void RefillFuel() {
        //monitoredEngine.currentFuel = monitoredEngine.maxFuel;
        fuelTank.RefillFuel();
        needToRefuel = false;
        fuelCriticalyLow = false;
    }
}
