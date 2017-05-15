#define mothershipVersion
#define TDVersion
#if mothershipVersion
using UnityEngine;
using System.Collections.Generic;



[@RequireComponent(typeof(UpdateQueue))]
public class Engine : EngineControlPanel {
    public float enginePower = 10.0f; // maximum engine power

    public UpdateQueue updating;
    protected bool devRunning = true;

    List<MessageCallback> onMoveMessages;

    public bool stopWhenArrive = true;
    // td version vars --------
    
    public Vector3 direction;
    Transform customPoint;
    public bool checkDistance = true;
    public Transform rotTransform;
    // engine will use last dist to check if it went over custom point because of high speed
    float lastDist;
    /// <summary>
    /// // just a flag for other scripts. lasts for 1 frame
    /// </summary>
    internal bool arrived;
    public bool useRotation = true;
    internal bool stop;


    void Start() {
        UpdateQueue updating = GetComponent<UpdateQueue>();
        OnStart();
    }

    protected virtual void OnStart() {
        updating.updateList[0] = EngineUpdate;

    }    
    
    void EngineUpdate() {

#if mothershipVersion
        // save position and execute move
        arrived = true;
        Vector3 lastPosition = transform.position;
        EnginePositionUpdate();
        Vector3 newPosition = transform.position;
        OnMove(lastPosition, newPosition);

        //#else        // TD verion, stops when arrives
        if (stopWhenArrive)
        {
            return;
        }
        if (stop)
        {
            print("arrived at end");
            return;
        }
        arrived = false;
        //MoveTowards(customPoint); 
        float nDist = Vector3.Distance(customPoint.position, transform.position);
        if (!(!checkDistance || ((checkDistance && nDist > 0.1f) || nDist > lastDist)))
        {
            arrived = true;
        }
        lastDist = nDist;
#endif //TDVersion
    }

    protected virtual void EnginePositionUpdate() {
        devRunning = false;
        if (!suspended) {
            devRunning = true;
            transform.Translate(Vector3.up * enginePower * Time.deltaTime);
        }
    }

    public void CancelRegistration(Transform source) {
        if (onMoveMessages == null) {
            return;
        }
        for (int i = 0; i < onMoveMessages.Count; i++) {
            if (onMoveMessages[i].target == source) {
                onMoveMessages.RemoveAt(i);
                i--;
            }
        }
    }

    public void RegisterOnMove(Transform source, System.Action<EngineControlPanel, Vector3, Vector3> callback) {
        if (onMoveMessages == null) {
            onMoveMessages = new List<MessageCallback>();
        }
        this.onMoveMessages.Add(new MessageCallback(source, callback));
    }

    protected void OnMove(Vector3 worldOldPos, Vector3 worldNewPos) {
        if (onMoveMessages != null) {
            for (int i = 0; i < onMoveMessages.Count; i++) {
                if (onMoveMessages[i] != null) {
                    onMoveMessages[i].callback(this, worldOldPos, worldNewPos);
                }
            }
        }
    }


    [System.Obsolete("this way, combined with transform.Translate(direction * speed); give REALLY smooth movement. Its little smiliar to lerp, " +
        "but better slowdown. Don't throw it away")]
    public void MoveTowards(Vector3 point)
    {

        direction = point - transform.position;

        this.customPoint.position = point;
    }
}
#elif TDVersion
using UnityEngine;
using System.Collections;

public class Engine : MonoBehaviour
{

    public float speed = 0.5f;
    public Vector3 direction;
    Transform customPoint;
    public bool checkDistance = true;
    public Transform rotTransform;
    // engine will use last dist to check if it went over custom point because of high speed
    float lastDist;
    /// <summary>
    /// // just a flag for other scripts. lasts for 1 frame
    /// </summary>
    internal bool arrived;
    public bool useRotation = true;
    internal bool stop;

    void Awake()
    {
        customPoint = new GameObject().transform;
        MoveTowards(customPoint.position);
    }

    // Update is called once per frame
    void Update()
    {

        if (stop)
        {
            print("arrived at end");
            return;
        }
        arrived = false;
        //MoveTowards(customPoint); 
        float nDist = Vector3.Distance(customPoint.position, transform.position);
        if (!checkDistance || ((checkDistance && nDist > 0.1f) || nDist > lastDist))
        {
            Vector3 dir = direction;

            transform.Translate(dir.normalized * speed * Time.deltaTime);
            if (useRotation)
            {
                rotTransform.right = customPoint.position - transform.position;
            }
            Debug.DrawLine(transform.position, customPoint.position);
            Debug.DrawRay(transform.position, direction);
        }
        else
        {
            arrived = true;
        }
        lastDist = nDist;
    }

    [System.Obsolete("this way, combined with transform.Translate(direction * speed); give REALLY smooth movement. Its little smiliar to lerp, " +
        "but better slowdown. Don't throw it away")]
    public void MoveTowards(Vector3 point)
    {

        direction = point - transform.position;

        this.customPoint.position = point;
    }

}
#endif
