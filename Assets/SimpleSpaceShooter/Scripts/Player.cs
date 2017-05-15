using UnityEngine;
using System.Collections;
using System;

public class Player : SimplestMovement {

    public bool mouseVersion = false;

	// Update is called once per frame
	new void Update () {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);


        float arrowInput = 0;
        if (!mouseVersion) {
            movementVertical.SetLerpTo(v, true);
            movementHorizontal.SetLerpTo(h, true);
            rotation.SetLerpTo(arrowInput, true);
        } else {
            transform.position = new Vector3(mouse.x, mouse.y, transform.position.z);
        }

        // get bounds within camera
        // TODO: save cam into var
        Camera camera = Camera.main;
        var bottomLeft = camera.ScreenToWorldPoint(Vector3.zero);
        var topRight = camera.ScreenToWorldPoint(new Vector3(
            camera.pixelWidth, camera.pixelHeight));

        var cameraRect = new Rect(
            bottomLeft.x,
            bottomLeft.y,
            topRight.x - bottomLeft.x,
            topRight.y - bottomLeft.y);
        boundsRect = cameraRect;

        base.Update(); // movement is applied at the end
    }

  
}


    /*
    // 0.5, 8, 0.5, 0.35
    // move by, rotate by, multiplie by curh and curv
    public float speed = 0.5f, steering = 8;

    // how fast is speed gained, 2 = 2x faster. if its greater than steering/speed, then move will be done in instant
    public float accelerationRate = 0.5f, mobilityRate = 0.35f;

    // original speed assigned at start of game
    internal float nonModifiedSpeed, nonModifiedSteering;

    float _lerptoH, _lerptoV;
    float htime, vtime;
    float lasth, lastv;

    float starth, startv;
    float curh, curv;

    public float lerptoH { get { return _lerptoH; } set { if (value != _lerptoV) lasth = _lerptoH; _lerptoH = value; } }
    public float lerptoV { get { return _lerptoV; } set { if (value != _lerptoV) lastv = _lerptoV; _lerptoV = value; } }

    public bool useMovement = true, useRotation = true;

    void Start() {
        
    }

    internal void HMove(float h) {
        lerptoH = Mathf.Clamp(h, -steering  , steering);// where to lerp

        starth = curh;// init where to begin lerping
        htime = 0;
    }

    internal void VMove(float v) {
        lerptoV = Mathf.Clamp(v, -speed, speed);

        startv = curv;
        vtime = 0;
    }

    /// <summary>
    /// Don't override it!
    /// 
    /// Moves unit around
    /// </summary>
    protected void Update() {
        //lerp values
        curh = Mathf.Lerp(starth, lerptoH, ((htime+steering)/2)/steering);// htime/speed because parameter is 0-1, and time goes up to steering. time+steering_/2 to get rid of negative part
        htime += Time.deltaTime * accelerationRate;

        curv = Mathf.Lerp(startv, lerptoV, ((vtime+speed)/2)/speed);
        vtime += Time.deltaTime * mobilityRate;

        // move/rotate object
        Vector3 move = new Vector3(0, curv) * speed;
        Vector3 rotation = new Vector3(0, 0, -curh) * steering;

        if (useMovement) transform.Translate(move);
        if (useRotation) transform.Rotate(rotation);
    }
    */
