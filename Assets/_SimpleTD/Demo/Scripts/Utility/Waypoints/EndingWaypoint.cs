using UnityEngine;
using System.Collections;

public class EndingWaypoint : Waypoint {

    public HpControl levelHp;

    public void ArrivedAtEnd(Engine engine) {
        Unit unit = engine.GetComponent<Unit>();
        levelHp.Damage(unit.livesWorth);
        GameObject.Destroy(unit.gameObject);
    }
}
