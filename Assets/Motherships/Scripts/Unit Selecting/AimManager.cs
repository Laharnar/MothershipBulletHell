using UnityEngine;
using System.Collections;

/// <summary>
/// This aim manager is specificaly made for player.
/// If you will want to have high level AI, use this same script, but separate UnitSelector and AIUnitSelector isntead of writing new aim manager for ai.
/// 
/// Select between manual and auto-aim on selected units and chose shooting direction or who target focus for those units.
/// 
/// Shooting into direction --
/// Once guns are selected, right clicking them will focus their priority aim in selected direction
/// 
/// Manual and auto-aim
/// For selected guns, 2 buttons show up, auto aim if some guns are on manual, and manual aim if some guns are on auto. Click buttons to change mode on guns.
/// 
/// </summary>
public class AimManager : MonoBehaviour {

    UnitSelector selectedUnits;

	// Use this for initialization
	void Start () {
        selectedUnits = GetComponent<UnitSelector>();
	}
	
	// Update is called once per frame
	void Update () {
        // right click sets aim point for selected cannons
        if (Input.GetMouseButtonUp(1)) {
            if (selectedUnits.areUnitsSelected) {
                Gun[] guns = selectedUnits.GetGuns;

                for (int i = 0; i < guns.Length; i++) {
                    Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    RaycastHit2D raycast2d= Physics2D.Raycast(world, Vector2.zero, Mathf.Infinity, 1 << LayerMask.NameToLayer("EnemyTarget"));

                    // If enemy unit was selected, target it. If not, shoot in that direction
                    if (raycast2d.transform) {
                        guns[i].FocusFire(raycast2d.transform);
                    } else {
                        guns[i].FocusFire(world);
                    }
                }

                LaserGun[] lasers = selectedUnits.GetLasers;

                for (int i = 0; i < lasers.Length; i++) {
                    Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    RaycastHit2D raycast2d = Physics2D.Raycast(world, Vector2.zero, Mathf.Infinity, 1 << LayerMask.NameToLayer("EnemyTarget"));

                    // If enemy unit was selected, target it. If not, shoot in that direction
                    if (raycast2d.transform) {
                        lasers[i].FocusFire(raycast2d.transform);
                    } else {
                        lasers[i].FocusFire(world);
                    }
                }
            }
        }
	}
}
