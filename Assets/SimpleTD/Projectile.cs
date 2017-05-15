using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

    public float speed;
    public Vector3 flyInDirection = Vector2.right;
    public int damage;

    // after how many hits will bullet be destroyed
    public int maxHitCount = 1;
    int hitCount=0;

    // allows projectile to fly only to the target, and not infinite
    LimitedRange aimDirectly;
    bool initialized=false;


	// Use this for initialization after initialization
	public void OnInstantiated (Vector3 targetPosition) {
        aimDirectly = new LimitedRange();
        aimDirectly.targetPosition = targetPosition;
        if (!GetComponent<Collider2D>().isTrigger) {
            Debug.Log("ERROR!!!!");
        }
        initialized = true;
	}
	
	// Update is called once per frame
	void Update () {
        if (!initialized)
        {
            Debug.Log("ERROR!!!!");
        }
        transform.Translate(flyInDirection.normalized * speed * Time.deltaTime);
        if (Vector3.Distance(aimDirectly.targetPosition, transform.position) < 0.05f)
        {
            Destroy(gameObject);
        }
	}

    void OnTriggerEnter2D(Collider2D other) {
        ProjectileAcceptor hitUnit = other.GetComponent<ProjectileAcceptor>();
        if (hitUnit) {
            hitUnit.GetDamage(this);
            hitCount++;
        }
        if (hitCount >= maxHitCount)
        {
            Destroy(gameObject);
        }
    }
}

class LimitedRange
{
    internal Vector3 targetPosition;
}