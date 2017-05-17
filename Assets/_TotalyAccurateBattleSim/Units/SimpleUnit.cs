using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// Contains besic movement for any kind of extended ai.
/// Rotates and moves based on smoothed movement - horizontal and vertical
/// Smiliar to player's version of horizontal and vertical.
/// 
/// Allows forward, back movement together with left and right rotation, with instant or slowdown effect idle
/// </summary>
public class SimpleUnit : MonoBehaviour {

    public float acceleration = 1; // speed mutiplier, 2=2x faster
    public float mobility = 1; // rotation multiplier, 2=2x faster

    /// <summary>
    /// Note: permanently modifying speed might require updating original values
    /// </summary>
    public float speed = 2;
    public float steering = 20;

    // original speed assigned at start of game
    internal float nonModifiedSpeed;
    internal float nonModifiedSteering;

    string movementState = "idle";
    string rotationState = "idle";

    // which object to move/rotate, transform is default
    public Transform moved;
    public Transform rotated;

    /// <summary>
    /// Callback that tells how much object moved
    /// Connect it to your own classes that need this info
    /// 0: movement change, 1: rotation change
    /// </summary>
    internal Action<Vector3, Vector3> onMoveBy;

    public bool useMovement = true;
    public bool useRotation = true;

    protected float lastSteering;

    // how much is actual speed different from original -- normalized
    public float speedDifference { get { return (nonModifiedSpeed - speed)/ nonModifiedSpeed; } }

    internal Vector3 fullPossibleMove = Vector3.zero;
    internal Vector3 fullPossibleRotation = Vector3.zero;

    internal float getVertical { get { return vertical; } }
    internal float getHorizontal { get { return horizontal; } }

    #region Set direction properties
    private float _rotateToHorizontal = 0;
    private float rotateToHorizontal
    {
        get
        { return _rotateToHorizontal; }
        set
        {
            engines["goingLeft"] = value < 0;
            engines["goingRight"] = value > 0;
            _rotateToHorizontal = value;
        }
    }


    private float _moveToVertical = 0;
    /// <summary>
    /// -1 to 1, how much forward it will move.
    /// </summary>
    private float targetMoveToVertical
    {
        get { return _moveToVertical; }
        set
        {
            engines["goingForward"] = value > 0;
            _moveToVertical = value;
        }
    }

    float _horizontal;
    /// <summary>
    /// -1 to 1, rotation left and right.
    /// </summary>
    float horizontal
    {
        get { return _horizontal; }
        set
        {
            engines["acceleratingLeft"] = rotateToHorizontal < value;
            engines["acceleratingRight"] = rotateToHorizontal > value;
            engines["decceleratingLeft"] = rotateToHorizontal > value;
            engines["decceleratingRight"] = rotateToHorizontal < value;
            _horizontal = value;
        }
    }
    float _vertical;

    float vertical
    {
        get { return _vertical; }
        set
        {
            engines["acceleratingForward"] = value < targetMoveToVertical;
            engines["decceleratingForward"] = value > targetMoveToVertical;
            engines["areAllOff"] = !engines["goingForward"] && !engines["goingLeft"] && !engines["goingRight"];
            _vertical = value;
        }
    }
    #endregion

    // other classes use these values via properties
    // they are set asvertical and horizontal are set
    internal Dictionary<string, bool> engines = new Dictionary<string, bool>()
    {
        { "acceleratingForward", false },
        { "acceleratingLeft", false },
        { "acceleratingRight", false },
        { "goingForward", false }, // decelerating = going forward:false
        { "goingLeft", false },
        { "goingRight", false },
        { "decceleratingForward", false },
        { "decceleratingLeft", false },
        { "decceleratingRight", false },
        { "areAllOff", false },
    };

    public enum Moves
    {
        forward,
        back,
        left,
        right,
        idle
    }
    
    protected void Awake()
    {
        lastSteering = steering;
        if (!moved)
            moved = transform;
        if (!rotated)
            rotated = transform;

        nonModifiedSpeed = speed;
        nonModifiedSteering = steering;

        StartCoroutine(RotationLerp());
        StartCoroutine(InterpolateVerticalLoop());
    }

    /// <summary>
    /// Executes rotation and movement
    /// </summary>
    void Update () {
        fullPossibleMove = new Vector3(0, vertical) * Time.deltaTime * nonModifiedSpeed;
        fullPossibleRotation = new Vector3(0, 0, horizontal) * Time.deltaTime * nonModifiedSteering;
        Vector3 move = new Vector3(0, vertical) * Time.deltaTime * speed;
        Vector3 rotation = new Vector3(0, 0, horizontal) * Time.deltaTime * steering;

        if (useMovement) moved.Translate(move);
        if (useRotation) rotated.Rotate(rotation);

        if (onMoveBy != null)
        {
            onMoveBy(useMovement ? move : new Vector3(), useRotation ? rotation : new Vector3());
        }
	}

    /// <summary>
    /// Modify original speed by some amount.
    /// </summary>
    /// <param name="speedMod">normalized speed mod</param>
    internal void ModifySpeed(float speedMod)
    {
        speed = nonModifiedSpeed * speedMod;
    }

    public void Idle(bool instantlyResetMove = false, bool instantlyResetRot = true
        , bool resetMovement = true, bool resetRotation = true)
    {// slowly eases into idle
        // movement reset
        if (resetMovement)
        {
            if (instantlyResetMove) vertical = 0;
            targetMoveToVertical = 0;
            movementState = "idle";
        }
        // rotation reset
        if (resetRotation)
        {
            if (instantlyResetRot) horizontal = 0; 
            rotateToHorizontal = 0;
            rotationState = "idle";
        }
    }

    public void ExecuteMove(Moves moveType,
        bool overrideOtherTypeMoves = false, 
        bool stopWhenDone = false, 
        bool resetFirst = false 
        )
    {
        string moveKeyword = moveType.ToString();
        //Debug.Log(keyword);
        if (moveType == Moves.forward)
        {
            FireMove(Forward, movementState, moveKeyword, overrideOtherTypeMoves, stopWhenDone, resetFirst);
        }
        if (moveType == Moves.back)
        {
            FireMove(Back, movementState, moveKeyword, overrideOtherTypeMoves, stopWhenDone, resetFirst);
        }
        if (moveType == Moves.left)
        {
            FireMove(TurnLeft, rotationState, moveKeyword, overrideOtherTypeMoves, stopWhenDone, resetFirst);
        }
        if (moveType == Moves.right)
        {
            FireMove(TurnRight, rotationState, moveKeyword, overrideOtherTypeMoves, stopWhenDone, resetFirst);
        }
        if (moveType == Moves.idle)
        {
            Idle();
        }
    }

    private void FireMove(Action<bool, bool, bool> action, string movementState, string keyword, bool overridePreviousAction = true, bool stopWhenDone = false, bool resetFirst = false)
    {
        if (movementState == keyword) return; // only set forward once
        else movementState = keyword;
        action(overridePreviousAction, stopWhenDone, resetFirst);
    }

    void SetVerticalMoves(int vdir, bool overrideRotation = true, bool stopWhenDone = false, bool resetFirst = false)
    {
        if (overrideRotation)
        {
            horizontal = 0;
        }

        if (resetFirst)
        {
            vertical = 0;
        }
        targetMoveToVertical = vdir;
    }

    public void Forward(bool overrideRotation = false, bool stopWhenDone = false, bool resetFirst = false)
    {
        SetVerticalMoves(1, overrideRotation, stopWhenDone, resetFirst);
    }

    void Back(bool overrideRotation = false, bool stopWhenDone = false, bool resetFirst = false)
    {
        SetVerticalMoves(-1, overrideRotation, stopWhenDone, resetFirst);
    }

    void SetHorizontalMoves(int hdir, bool overrideMovement = false, bool stopWhenDone = false, bool resetFirst = false)
    {
        hdir *= -1;
        if (overrideMovement)
        {
            vertical = 0;
        }
        if (resetFirst)
        {
            horizontal = 0;
        }

        rotateToHorizontal = hdir;
    }

    void TurnLeft(bool overrideMovement = false, bool stopWhenDone = false, bool resetFirst = false) {
        SetHorizontalMoves(-1, overrideMovement, stopWhenDone, resetFirst);
    }

    void TurnRight(bool overrideMovement = false, bool stopWhenDone = false, bool resetFirst = false)
    {
        SetHorizontalMoves(1, overrideMovement, stopWhenDone, resetFirst);
    }

    /// <summary>
    /// rotation loop that takes care of lerping of horizontal value
    /// 
    /// change 'rotateToHorizontal' to change lerp target.
    /// </summary>
    /// <param name="rotateToHorizontal"></param>
    /// <returns></returns>
    IEnumerator RotationLerp(float rotateToHoriz = 0)
    {
        rotateToHorizontal = rotateToHoriz;
        while (true)
        {
            float rtime = ((horizontal + 1) / 2) * mobility;

            float i = 0.0f;
            float rate = 1.0f / rtime;
            float startValue = horizontal;
            float lastRotationTo = rotateToHorizontal;
            while (i < 1.0)
            {
                if (lastRotationTo != rotateToHorizontal)// make sure calculations are up to date, reset them
                    break;// reset calculations if target lerp changes

                i += Time.deltaTime * rate;
                horizontal = Mathf.Lerp(startValue, rotateToHorizontal, i);
                yield return null;
            }
        }
    }


    IEnumerator InterpolateVerticalLoop(float startingMove = 0)
    {
        targetMoveToVertical = startingMove;
        while (true)
        {
            // calculate in how many steps should rotation occur 
            float vtime = ((vertical + 1) / 2) * acceleration;
            
            // lerp the vertical value over time
            float time = 0.0f;
            float rate = 1.0f / vtime;
            float startValue = vertical;
            float lastMoveTo = targetMoveToVertical;
            while (time < 1.0)
            {
                // don't count rotations if they are the same
                if (lastMoveTo != targetMoveToVertical)
                    break;

                time += Time.deltaTime * rate;
                vertical = Mathf.Lerp(startValue, targetMoveToVertical, time);

                yield return null;
            }
        }
    }


}
