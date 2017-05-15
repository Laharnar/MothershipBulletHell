using UnityEngine;
using System.Collections;

public class StraightBullet : LinearProjectile {


    void Update() {
        //type - bullet
        
        if (!noMovement) {
            
            transform.Translate(direction * speed * Time.deltaTime);
        }
    }
}
