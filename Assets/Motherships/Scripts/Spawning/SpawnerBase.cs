using UnityEngine;
using System.Collections;

public class SpawnerBase : MonoBehaviour {

    public Transform [] spawnPoints;

    public bool[] fired;

    public int[] clipSize;

    public float clipReloadTime;// cant fire while reloading
}
