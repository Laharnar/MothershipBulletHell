
using UnityEngine;
using System.Collections;

[@RequireComponent(typeof(FireControl))]
[System.Obsolete("Dont need this type of script for now. Firing is done in different way, with multiiple small scripts.")]
public class ArcherTower : Tower {
#if old_approach
    public FireControl fire;

    void Update() {
        // just for test
        if (fire.fireDetection.target)
        transform.LookAt(fire.fireDetection.target);
    }

    void OnTriggerEnter2D(Collider2D other) {
        fire.fireDetection.OnTriggerEnter(other);
    }

    void OnTriggerExit2D(Collider2D other) {
        fire.fireDetection.OnTriggerExit(other);
    }


    public void Init() {
        if (fire == null) {
            fire= GetComponent<FireControl>();
            print("init");
        }
        fire.Init();
    }
#endif
}