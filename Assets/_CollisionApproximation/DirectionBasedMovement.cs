using UnityEngine;
using System.Collections;

/// <summary>
/// change direction every x seconds
/// 
/// instant/smooth version
/// </summary>
public class DirectionBasedMovement : SimplestMovement {

    public bool smooth = false;
    
    public float[] directions;
    int activeDir;
    public float rate = 1f;

    // Use this for initialization
    void Start () {

        StartCoroutine(ChangeDir());
    }
	
    IEnumerator ChangeDir() {
        while (true) {
            rotateTarget.rotation = Quaternion.Euler(0, 0, directions[activeDir]);
            activeDir = ++activeDir % directions.Length;
            yield return new WaitForSeconds(rate);
        }
    }
	// Update is called once per frame

        //transform.rotation = lookAt;



        /* Vector3 targetDir = targetPoint - transform.position;
         //steer = (goRight + goUp) / 2;
         angle = Vector3.Angle(targetDir, transform.up);

         // dot = Vector3.Dot(transform.position, targetDir.normalized);
         goRight = Vector3.Dot(targetDir, (transform.right).normalized);
         goUp = Vector3.Dot(targetDir, (transform.up).normalized);

         if (goRight < 0) { // to left

         } else if (goRight > 0) { // to right

         } else if (angle > 0) { // directly behind, to right
             goRight = 1;
         }

         //dir = (1+goRight) * (1+goUp)/2;
         final = (goRight) * angle;// how large the movement has to be

         transform.Rotate(0, 0, -final/90);
         */

        //transform.Rotate(0, 0, Vector3.Angle(targetPoint - transform.position, transform.up));
        /*if(dot < 0) {
            rotation.SetLerpTo(-1);
        }

        if (dot > 0) {
            rotation.SetLerpTo(1);
        }*/

}
