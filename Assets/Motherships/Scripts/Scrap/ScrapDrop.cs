using UnityEngine;
using System.Collections;

//  How to remove:
/// <summary>
/// Object that produces some scrap when destroyed.
/// 
/// Relies on 'ScrapManager' singleton to send scrap to
/// 
/// Put it on every enemy ships that give scrap.
/// 
/// Date added: 19.8.2016
/// 
/// Edit 2.0: 29.8.2016
/// Doesn't require ScrapManager in scene anymore. Now drop is sent to manager who does with it whatever it does(not guaranted that it's in scene).
/// 
/// </summary>
/// <remarks>
/// How to remove this drop system:
/// - remove this script from all objects in project. Since it uses delegates on death, this is all it takes
/// </remarks>
public class ScrapDrop : MonoBehaviour {

    public int dropAmount = 1;

    public Spaceship target;//which spaceship this drop refrers to
    public ShipInfo targetInfo;

	// Use this for initialization
	void Start () {
        if (targetInfo) {
            if (targetInfo.explodeHp)
                targetInfo.explodeHp.onDestroyed += Drop;
        } else Logger.AddLog("Optinal:Assign explode hp script for drop.", this);
        target = GetComponent<Spaceship>();
        this.InspectorNullComponentWarning(target, "On-drop target isn't assigned.");
        //if (target) [obsolete]
        //    target.RegistedOnHpZero(Drop);
	}
	
    /// <summary>
    /// Function drops scrap parts or gives the amount to player.
    /// </summary>
	public void Drop() {
        ScrapManager.RegisterScrapDrop(this, dropAmount);
    }
}
