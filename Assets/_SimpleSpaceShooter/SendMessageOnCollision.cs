using UnityEngine;
using System.Collections;

public class SendMessageOnCollision : CollisionReceiver {

    public string functionName;
    public string value;

    public override void OnCollideYourFaction(ProxyCollision other) {
        SendMessage(functionName, value);
    }
}
