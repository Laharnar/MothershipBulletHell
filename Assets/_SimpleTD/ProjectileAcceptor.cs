using UnityEngine;
using System.Collections;

/// <summary>
/// Connects getting damage and hp
/// </summary>
public class ProjectileAcceptor : MonoBehaviour {

    public HpControl hp;
	
	// Update is called once per frame
	public void GetDamage (Projectile source) {
        if (hp)
        {
            hp.Damage(source.damage);
        }
    }
}
