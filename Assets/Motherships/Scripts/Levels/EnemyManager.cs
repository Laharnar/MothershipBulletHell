using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour {

    static EnemyManager sm;

    public List<GameObject> enemies;

    public int MainEnemyCount { get { return enemies.Count; } }

    static string warning = "";

    void Awake() {
        if (sm) {
            Destroy(sm.gameObject);
        }
        sm = this;

        enemies = new List<GameObject>();
        GameObject[] es = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < es.Length; i++) {
            if (es[i].GetComponent<Bomber>() || es[i].GetComponent<AssaultCraft>()) {
                continue;
            }
            enemies.Add(es[i]);
        }
    }

    /// <summary>
    /// call on destroyed unit rught before getting destroyed
    /// </summary>
    public void BeforeEnemyDestroyed(Transform enemy) {
        if (enemies.Contains(enemy.gameObject)) {// check if it contains it, since it doesn't have player's ship in it
            enemies.Remove(enemy.gameObject);
        }
        if (enemies.Contains(null)) {
            enemies.Remove(null);
        }
    }

    internal static void ShipDown(Transform ship) {
        if (ship.tag == "Player") {
            EndLevel el = GameObject.FindObjectOfType<EndLevel>();
            if (el) el.PlayerDownLevel();
        }
        if (sm) {
            sm.BeforeEnemyDestroyed(ship);
        } else {
            if (warning=="") {
                Debug.LogWarning("No 'Enemy manager' in scene. Can't registere global ship was destroyed");
                warning = "No 'Enemy manager' in scene. Can't registere global ship was destroyed";
            }
        }
    }
}
