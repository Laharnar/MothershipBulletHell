using UnityEngine;
using System.Collections;

/// <summary>
/// Half collision/half trigger version of bullet
/// </summary>
public class Bullet : PooledMonoBehaviour {

    static GameObject tmpBulletParent;// temporary generated object

    string alliance;// flag has to be saved in case source is already destroyed

    public int bulletDamage = 1;
    public float bulletLife = 5;

    public Rigidbody2D rig2d;

    public bool noMovement = false;
    public float speed = 10;
    Vector3 direction = Vector2.up;

    float timeout;
    bool init = false;

    private void Awake() {
        if (tmpBulletParent == null) {
            tmpBulletParent = new GameObject("[pool]bullets");
        }
        transform.parent = tmpBulletParent.transform;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sourceFlag">who fired bullet</param>
    public void InitBullet(string sourceFlag) {
        alliance = sourceFlag;
        timeout = Time.time+bulletLife;
        init = true;
    }

    void Update() {
        //type - bullet
        if (init) {
            if (!noMovement) {
                transform.Translate(direction * speed * Time.deltaTime);
            }
            if (Time.time > timeout) {

                Destroy(gameObject);
            }
        }
    }

    new void Destroy(Object go) {
        init = false;
        Destroy(go, InstancePool.PoolingMode.Move);
    }

    void OnCollisionEnter2D(Collision2D other) {
        CollideDamage(other.transform);
    }
    
    void OnTriggerEnter2D(Collider2D other) {
        CollideDamage(other.transform);
    }

    private void CollideDamage(Transform other) {
        ShipInfoProxy ship = other.GetComponent<ShipInfoProxy>();
        if (ship && ship.source.flag.alliance != alliance) {

            ship.source.hp.Damage(bulletDamage);
            //Debug.Log("bullet by '"+ alliance+" destroyed after hitting someone from '"+ship.source.flag.alliance);
            Destroy(gameObject);
        }
    }
}
