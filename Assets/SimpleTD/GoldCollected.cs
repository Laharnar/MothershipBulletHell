using UnityEngine;
using System.Collections;

public class GoldCollected : MonoBehaviour {

    static GoldCollected goldCollected;

    public int gold = 0;

	// Use this for initialization
	void Start () {
        goldCollected = this;
	}

    public static void AddGold(int g)
    {
        if (goldCollected == null)
        {
            goldCollected = new GameObject().AddComponent<GoldCollected>();
        }
        goldCollected.gold += g;
    }
}
