using UnityEngine;
using System.Collections;

public class GroupTurret : UnitInfo {

    static GroupTurret[] turrets = new GroupTurret[10000];
    static int lastTurret = 0;

    public Transform rotationTarget;
    public Transform target;

    public BigParentCollision watcher;

    // Use this for initialization
    protected new void Start() {
        StartCoroutine(EnableBehaviours());
    }

    private IEnumerator EnableBehaviours() {
        float time = UnityEngine.Random.Range(1, Time.time);
        float t = Time.time + time;
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

        turrets[lastTurret++] = this;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (Time.time < 0.5) {
            return;
        }

        // on trigger set target
        target = other.transform;

        //rotationTarget.LookAt(other.transform, Vector2.up);
        //rotationTarget.rotation = Quaternion.Euler(rotationTarget.eulerAngles.x, rotationTarget.eulerAngles.y, 0);

    }

    void Update() {
        rotationTarget.LookAt(target, Vector3.back);
    }
}
