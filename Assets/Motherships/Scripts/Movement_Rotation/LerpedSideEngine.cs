
using UnityEngine;
public class LerpedSideEngine : SideEngine {

    public float mobility = 25.0f;//mobility in 360 degrees per mobility seconds
    public UpdateQueue linkedUpdate;

    public bool restrictRotation { get; set; }

    Vector3 targetPosition;

    void Start() {
        linkedUpdate.updateList[1] = Steering;
    }

    protected override void Steering() {
        if (restrictRotation) {
            mobility = mobility * 0.35f;// increase time it takes to make the turn
        }
        
        Quaternion newRotation = Quaternion.LookRotation(transform.position - targetPosition, Vector3.forward);
        newRotation.x = 0.0f;
        newRotation.y = 0.0f;
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * mobility);
        
    }
}
