using UnityEngine;
using System.Collections;
using System;

public class FuelTank : MonoBehaviour {

    public float maxFuel = 20;
    float fuel = 0;
    public float fuelUse = 1;

    public SimpleUnit fuelUser;

    /// <summary>
    /// normalized percentages of fuel left
    /// </summary>
    public float currentFuel { get { return fuel / maxFuel; } }

    // Use this for initialization
    void Start () {
        fuel = maxFuel;
        fuelUser.onMoveBy += MoveUpdate;
	}
	
    void MoveUpdate(Vector3 mov, Vector3 rot) {
        // how much unit moved vs how much it would move at max speed, including how long the frame was
        Vector3 move = (fuelUser.fullPossibleMove - mov);
        Vector3 diff = new Vector3(move.x / fuelUser.fullPossibleMove.x,
            move.x / fuelUser.fullPossibleMove.y);
        float movePercentage = Mathf.Sqrt(diff.x * diff.x+ diff.y * diff.y);
        ConsumeFuel(fuelUse * movePercentage * Time.deltaTime);
    }

    private void ConsumeFuel(float amount) {
        fuel = Mathf.Clamp(fuel-amount, 0, maxFuel);
    }

    internal void RefillFuel() {
        fuel = maxFuel;
    }
}
