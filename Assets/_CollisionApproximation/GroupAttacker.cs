using UnityEngine;
using System.Collections;
using System;

// note: disable collisions at startup by hand
// choses some target in range and attacks it.
public class GroupAttacker : UnitInfo {

    static GroupAttacker[] attackers = new GroupAttacker[10000];
    static int lastAttacker = 0;

    static Transform[] targets = new Transform[10000];
    static int lastTarget = 0;


    Transform rotationTarget;

    Transform target;
    public BigParentCollision watcher;

    float t;

	// Use this for initialization
	protected new void Start () {
        StartCoroutine(EnableBehaviours());
    }

    private IEnumerator EnableBehaviours() {
        float time = UnityEngine.Random.Range(1, Time.time);

        t = Time.time + time;
        while (Time.time < t) {
            yield return null;
        }
        base.Awake();
        //gameObject.AddComponent<BoxCollider2D>();
        if (GetCompt<SpriteRenderer>() != null) {
            GetLastCompt<SpriteRenderer>().enabled = true;
        }
        
        if (GetCompt<CircleCollider2D>() != null) {
            GetLastCompt<CircleCollider2D>().enabled = true;
        }


        if (watcher == null) {
            watcher = transform.parent.GetComponent<BigParentCollision>();
        }
        watcher.RegisterUnit(this);

        attackers[lastAttacker++] = this;

    }

    void OnTriggerEnter2D(Collider2D other) {
        if (Time.time < 0.5+t) {
            return;
        }

        if (other.transform.name == "target") {
            if (target == null)
                target = other.transform;
            targets[lastTarget++] = other.transform;
        }
    }
    
    void Update() {
        if (target) {
            rotationTarget.LookAt(target, Vector3.back);
            transform.Translate(Vector3.up);
            if (Vector3.Distance(transform.position, target.position) < 5) {
                Destroy(target.gameObject);
                Destroy(gameObject);

                // since its suicider, it doesnt need more targets
                /*if (lastAttacker !=  0) {
                    for (int i = 0; i < targets.Length; i++) {
                        if (targets[i] != null) {
                            target = targets[i];
                        }
                    }
                }*/
            }
        }
    }
}
