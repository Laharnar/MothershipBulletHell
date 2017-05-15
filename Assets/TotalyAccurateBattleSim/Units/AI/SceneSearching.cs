using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public static class SceneSearching
{

    // move this to other script
    internal static void Idle(SimpleUnit getSource,
       bool instaResetMov = false, bool instaResetRot = true,
       bool resetMovement = true, bool resetRotation = true)
    {
        getSource.Idle(instaResetMov, instaResetRot, resetMovement, resetRotation);
    }



    public static IEnumerator AttackWithRefuel
        (Transform transform,
        Transform possibleTarget,
        float closestDistance,
        LerpedSideEngine sideEngine,
        float mobility,
        RefuelControl enginesMonitor,
        GunControl gunControl,
        Action onLeft,
        Action onRight)
    {
        // edit: active target isnt private to class but to function
        Transform activeTarget = possibleTarget;
        while (activeTarget != null)
        {
            if (enginesMonitor.needToRefuel)
            {
                break;
            }

            //enginesMonitor.monitoredEngine.Run(true);
            gunControl.FireForwardGuns();

            gunControl.FireRotatingGuns(activeTarget);

            if (Vector3.Distance(activeTarget.position, transform.position) > closestDistance)
            {
                MovementCommands.SmoothRotateTowards(transform, activeTarget.position, mobility, sideEngine);//  function() {Steering(targetPosition)}

            } else
            {
                gunControl.CancelFireForwardGuns();
                yield return Avoid(transform, activeTarget, onLeft, onRight);
            }
            yield return null;
        }
        gunControl.CancelFireForwardGuns();

        //enginesMonitor.monitoredEngine.Run(false);
    }

    public static Collider2D[] ScanArea(Vector3 position, float radius)
    {
        return Physics2D.OverlapCircleAll(position, radius);
    }

    internal static Queue<Vector3> CreateCircularPathAround(this Transform transform, Transform target, int samples, float radiusOfPath,
        bool constructCounterClockwise = false)
    {
        if (samples == 0)
        {
            return null;
        }

        int minDisti = -1;
        float minDistToTransform = float.MaxValue;

        // clockwise path
        Vector3[] temp = new Vector3[samples];
        for (int i = 0; i < samples; i++)
        {
            var a = (2 * Mathf.PI / samples) * i;
            var x = target.position.x + radiusOfPath * Mathf.Cos(a);
            var y = target.position.y + radiusOfPath * Mathf.Sin(a);
            var vec = new Vector2(x, y);

            temp[i] = vec;

            // get min
            var dist = Vector3.Distance(vec, transform.position);
            if (dist < minDistToTransform)
            {
                minDisti = i;
                minDistToTransform = dist;
            }
        }

        // turn around into counterclockwise if necessary
        if (constructCounterClockwise)
        {
            // len/2 goes up to middle, inclusive
            for (int i = 0; i < temp.Length/2; i++)
            {
                var val1 = temp[temp.Length-i-1];
                temp[temp.Length - i - 1] = temp[i];
                temp[i] = val1;
            }
        }
        
        // construct path to start at secondpoint in circle, this insures it wont go in reverse because first point is too close
        // you can fix that by checking if point is in front of transform
        Queue<Vector3> result = new Queue<Vector3>();
        for (int i = minDisti+1; i != minDisti; i=(i+1)% temp.Length)
        {
            Debug.DrawLine(temp[i], temp[(i+temp.Length-1)%temp.Length], Color.red, 10);
            result.Enqueue(temp[i]);
        }

        return result;
    }





    /// <summary>
    /// Avoid the target to certain distance by going left or right
    /// Doesn't check if there is no movement.
    /// You can do same version witout coroutine
    /// 
    /// This is tehnicaly beahviour
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="activeTarget"></param>
    /// <param name="avoidToDistance"></param>
    /// <returns></returns>
    public static IEnumerator Avoid(Transform transform,
        Transform target, Action OnLeft, Action OnRight, float avoidToDistance = 1
        )
    {
        string state = "";
        //rotate away for a while. its still going forward
        while (true)
        {
            if (Avoid(transform, target.position, avoidToDistance, ref state))
            {
                if (state == "left")
                {
                    // TODO: state = left
                    //Left();
                    OnLeft();
                }
                if (state == "right")
                {
                    //Right();
                    OnRight();
                }
            } else
            {
                break;
            }
            yield return null;
        }
    }

    /// <summary>
    /// sets state to left or right while if target is too close
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="target"></param>
    /// <param name="avoidToDistance"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public static bool Avoid(Transform transform,
        Vector3 target, float avoidToDistance
        , ref string state)
    {
        // avoid direction is chosen randomly based on time.
        // todo: you should chose it on min angle avoid target
        int choice = ((int)Time.time) % 2;
        state = "";

        float dist = Vector3.Distance(target, transform.position);
        if (dist > avoidToDistance)
        {
            return false;
        }

        if (choice == 0)
        {
            // TODO: state = left
            state = "left";
        } else
        {
            state = "right";
        }
        return true;
    }

    /// <summary>
    /// Returns first enemy scanned based on alliances
    /// -Previous version was based on spaceships-
    /// </summary>
    /// <param name="scanRange"></param>
    /// <param name="alliance"></param>
    /// <param name="drawRays"></param>
    /// <returns></returns>
    public static Collider2D Collision2DScan(this Transform transform, float scanRange, string alliance, bool drawRays = false)
    {
        Collider2D[] scanned = Physics2D.OverlapCircleAll(transform.position, scanRange, 1 << LayerMask.NameToLayer("Scan"));
        Collider2D enemy = null;
        for (int i = 0; i < scanned.Length; i++)
        {
            Alliance sp = scanned[i].transform.root.GetComponent<Alliance>();
            if (sp && sp.alliance != alliance)
            {
                enemy = scanned[i];
                break;
            }
        }
        if (drawRays)
        {
            Debug.DrawRay(transform.position - (Vector3.up + Vector3.right) * scanRange, (Vector3.up + Vector3.right) * 2 * scanRange);
            Debug.DrawRay(transform.position - (Vector3.up + Vector3.left) * scanRange, (Vector3.up + Vector3.left) * 2 * scanRange);
        }
        return enemy;
    }

    internal static void MoveTo(Transform transform, SimpleUnit getSource, Vector3 target, float howNear = 0.1f, float nearSpeedMod = 1, float farSpeedMod = 1,
        float nearbyRange = 1, float nearbyToTargetSpeedMod = 1)
    {
        howNear = Mathf.Clamp(howNear, 0, float.MaxValue);
        nearbyRange = Mathf.Clamp(nearbyRange, howNear, float.MaxValue);

        getSource.ModifySpeed(farSpeedMod);
        // use custom or default distance calculation
        float dist = DefaultDistance(transform, target);
        if (dist < nearbyRange)
        {
            getSource.ModifySpeed(nearbyToTargetSpeedMod);
        }
        if (dist > howNear)
        {
            GoTo(transform, getSource, target + Vector3.up);
        } else
        {
            //Idle();
            Idle(getSource);
            getSource.steering = getSource.nonModifiedSteering;
            //getSource.Idle();
        }
    }

    internal static IEnumerator MoveToPos(Transform transform, SimpleUnit getSource, Vector3 target, bool idleAtEnd, float howNear = 0.1f, float nearSpeedMod = 1, float farSpeedMod = 1,
        float nearbyRange = 1, float nearbyToTargetSpeedMod = 1)
    {
        nearbyRange = Mathf.Clamp(nearbyRange, howNear, nearbyRange);
        while (true)
        {
            howNear = Mathf.Clamp(howNear, 0, float.MaxValue);
            nearbyRange = Mathf.Clamp(nearbyRange, howNear, float.MaxValue);

            getSource.ModifySpeed(farSpeedMod);
            // use custom or default distance calculation
            GoTo(transform, getSource, target + Vector3.up);
            float dist = DefaultDistance(transform, target);
            if (dist < nearbyRange)
            {
                getSource.ModifySpeed(nearbyToTargetSpeedMod);
                if (dist > howNear)
                {
                    getSource.ModifySpeed(nearSpeedMod);
                } else
                {
                    if (idleAtEnd)
                    {
                        getSource.steering = getSource.nonModifiedSteering;
                        Idle(getSource);
                    }
                    break;
                }
            }
            yield return null;
        }
    }



    internal static IEnumerator FollowTarget_v2(Transform transform, SimpleUnit unit, Transform target, OverrideTravel travelInfo,
        bool stopWitoutTarget
        , float howNear = 0.1f, float nearSpeedMod = 1, float farSpeedMod = 1,
        float nearbyRange = 1, float nearbyToTargetSpeedMod = 1) {
        while (true) {
            if (travelInfo != null) {
                if (travelInfo.applySecondaryTravel) {
                    // Debug.DrawLine(transform.position, travelInfo.secondaryTravel, Color.green);
                    GoTo(transform, unit, travelInfo.secondaryTravel, howNear, nearSpeedMod, farSpeedMod, nearbyRange, nearbyToTargetSpeedMod);
                } else {
                    // Debug.DrawLine(transform.position, travelInfo.target.position, Color.red);
                    if (!Follow(transform, unit, travelInfo.target, stopWitoutTarget, howNear, nearSpeedMod, farSpeedMod, nearbyRange, nearbyToTargetSpeedMod))
                        break;
                }
            }else {
                Debug.Log("Apply travel info", transform);
            }
            yield return null;
        }
        // go idle, dont reset movement if it has to go forward
        unit.steering = unit.nonModifiedSteering;
        unit.Idle(resetMovement: stopWitoutTarget);
    }

    /// <summary>
    /// This version is ok for specific use
    /// </summary>
    /// <param name="getSource"></param>
    /// <param name="target"></param>
    /// <param name="stopWitoutTarget"></param>
    /// <param name="howNear"></param>
    /// <param name="distanceCalculation"></param>
    /// <returns></returns>
    public static IEnumerator FollowTarget(Transform transform, SimpleUnit getSource, Transform target, bool stopWitoutTarget
        , float howNear = 0.1f, float nearSpeedMod = 1, float farSpeedMod = 1,
        float nearbyRange = 1, float nearbyToTargetSpeedMod = 1, Func<Transform, float> distanceCalculation = null)
    {
        while (target)
        {
            if (!Follow(transform, getSource, target, stopWitoutTarget, howNear, nearSpeedMod, farSpeedMod, nearbyRange, nearbyToTargetSpeedMod, distanceCalculation))
                break;
            yield return null;
        }
        // go idle, dont reset movement if it has to go forward
        getSource.steering = getSource.nonModifiedSteering;
        getSource.Idle(resetMovement: stopWitoutTarget);
    }

    public static bool Follow(Transform transform, SimpleUnit getSource, Transform target, bool stopWitoutTarget, float howNear = 0.1f, float nearSpeedMod = 1, float farSpeedMod = 1,
    float nearbyRange = 1, float nearbyToTargetSpeedMod = 1, Func<Transform, float> distanceCalculation = null)
    {
        GoTo(transform, getSource, target, howNear, nearSpeedMod, farSpeedMod, nearbyRange, nearbyToTargetSpeedMod);
        // use custom or default distance calculation
        if (!((distanceCalculation != null && distanceCalculation(target) > howNear)
            || (distanceCalculation == null && DefaultDistance(transform, target.position) > howNear)))
            return false;
        return true;
    }

    internal static float DefaultDistance(Transform transform, Vector3 target)
    {
        return Vector3.Distance(transform.position, target);
    }

    /// <summary>
    /// Choses first unit in array that isn't itself
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="scene"></param>
    /// <returns></returns>
    public static Transform ChoseFirstTarget(Transform transform)//, UnitsInScene scene)
    {
        Transform target = null;
        Alliance[] units = UnitsInScene.GetAllUnits().ToArray();
        for (int i = 0; i < units.Length; i++)//scene.
        {
            if (units[i] == transform)
            {
                continue;
            }
            target = units[i].transform;
            break;
        }
        return target;
    }

    public static Transform ChoseClosestTarget(Transform transform, string flag)
    {
        Alliance[] units = UnitsInScene.GetEnemyShips(flag).ToArray();
        float minDist = float.MaxValue;
        int imin = -1;
        for (int i = 0; i < units.Length; i++)
        {
            if (units[i] == null)
            {
                continue;
            }
            if (units[i].alliance != flag)
            {
                if (minDist > Vector3.Distance(units[i].transform.position, transform.position))
                {
                    imin = i;
                }
            }
        }
        return imin == -1 ? null : units[imin].transform;
    }

    public static void GoTo(Transform transform, SimpleUnit getSource, Transform target, float howNear = 0.1f, float nearSpeedMod = 1, float farSpeedMod = 1,
    float nearbyRange = 1, float nearbyToTargetSpeedMod = 1)
    {
        if (!target)
        {
            return;
        }
        GoTo(transform, getSource, target.position, howNear, nearSpeedMod, farSpeedMod, nearbyRange, nearbyToTargetSpeedMod);
    }

    public static void TurnTowards(this Transform transform, AiInfo ai, Vector3 target) {

        float angle = Vector3.Angle(transform.up, target - transform.position);
        Vector3 axis = Vector3.Cross(transform.up, target - transform.position);

        ai.Get();
        float rotationAccuracyAngle = ai.rotationAccuracy;
        float rotationInnerAccuracyAngle = ai.innerAccuracy;
        float slowSteeringSpeed = ai.slowDownEffect;
        float regularSteeringSpeed = ai.nonModifiedSteering;
        SimpleUnit source = ai.source;

        if (angle > rotationAccuracyAngle) {
            source.steering = regularSteeringSpeed;
            MovementCommands.ExecuteRotationTo(source, axis.z);
        } else {
            source.steering = slowSteeringSpeed;
            if (angle > rotationInnerAccuracyAngle) {
                MovementCommands.ExecuteRotationTo(source, axis.z);
            } else {
                source.steering = regularSteeringSpeed;
                source.Idle(resetMovement: false); // reset rotation
            }
        }
    }


    /// <summary>
    /// travels to point update, goes into idle when if it arrived.
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="getSource"></param>
    /// <param name="target"></param>
    /// <param name="howNear"></param>
    /// <param name="nearSpeedMod"></param>
    /// <param name="farSpeedMod"></param>
    /// <param name="nearbyRange"></param>
    /// <param name="nearbyToTargetSpeedMod"></param>
    public static void GoTo(Transform transform, SimpleUnit getSource, 
        Vector3 target
     , float howNear = 0.1f, float nearSpeedMod = 1, float farSpeedMod = 1,
    float nearbyRange = 1, float nearbyToTargetSpeedMod = 1)
    {
        if (DefaultDistance(transform, target) > howNear) {
            TurnTowards(transform, getSource, target, 1f, getSource.steering, 0.01f, getSource.steering / 5);

            getSource.ExecuteMove(SimpleUnit.Moves.forward);
        }else
        getSource.Idle();
    }

    /// <summary>
    /// Rotates towards target 2 ways to rotate to target + smooth accurate rotation:
    /// when inside rotation rotate with normal speed
    /// when inside smooth rotation use slow speed
    /// 
    /// Example move params: 1f, getSource.steering, 0.01f, getSource.steering / 5
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="getSource"></param>
    /// <param name="target"></param>
    /// <param name="targetUp"></param>
    /// <param name="normalRotationAccuracy"></param>
    /// <param name="slowSpeed"></param>
    /// <param name="smoothRotationAccuracy"></param>
    /// <param name="drawLine"></param>
    public static void TurnTowards(Transform transform, SimpleUnit getSource, Vector3 target
        , float normalRotationAccuracy, float normalSpeed, float slowSpeed, float smoothRotationAccuracy)
    {
        float angle = Vector3.Angle(transform.up, target - transform.position);
        Vector3 axis = Vector3.Cross(transform.up, target - transform.position);

        if (angle > normalRotationAccuracy)
        {
            getSource.steering = normalSpeed;// getSource.nonModifiedRotation
            MovementCommands.ExecuteRotationTo(getSource, axis.z);
        } else
        {
            getSource.steering = slowSpeed;
            if (angle > smoothRotationAccuracy)
            {
                MovementCommands.ExecuteRotationTo(getSource, axis.z);
            } else
            {
                getSource.steering = getSource.nonModifiedSteering;
                getSource.Idle(resetMovement: false); // reset rotation
            }
        }
    }

    /// <summary>
    /// finds direction of target relative to the ship - left or right
    /// </summary>
    /// <returns></returns>
    public static int GetSideToTarget(Transform transform, Vector3 targetPosition)
    {
        var relativePoint = transform.InverseTransformPoint(targetPosition);
        if (relativePoint.x <= 0.0) // change into < later. and change 0.0s into something else to have better forward
            return 1;
        //print ("Object is to the left");
        else if (relativePoint.x > 0.0)
        {
            return -1;
            //print ("Object is to the right");
        } else
        {
            return 0;
            //print ("Object is directly ahead");
        }
    }


    /// <summary>
    /// Travel along the whole path
    /// </summary>
    /// <returns></returns>
    public static IEnumerator TravelPath(Transform transform, QuePath travel, Vector3 targetPos)
    {
        while (travel.NodeCount() > 0)
        {
            Debug.DrawLine(transform.position, targetPos, Color.red, 5);
            while (Vector3.Distance(targetPos, transform.position) > 1f)
            {
                yield return null;
            }
            targetPos = travel.NextNode().transform.position;// when arrive to it, go to next
        }
    }

    /// <summary>
    /// Chose path and go near first node
    /// </summary>
    /// <returns></returns>
    public static IEnumerator EnterPath(Transform transform, Vector3 targetPos,
        CircularPath targetsNodes, QuePath travel)
    {
        travel = targetsNodes.GetPath(targetsNodes.Closest(transform.position), 2);
        QuePath bakFullPath = travel;// unused

        targetPos = travel.NextNode().transform.position;//get first node
        Debug.DrawLine(transform.position, targetPos, Color.blue, 5);


        while (Vector3.Distance(targetPos, transform.position) > 1f)
        {
            yield return null;
        }
    }

    /// <summary>
    /// Calls updates while going between waypoins.
    /// Doesnt actualy move, just a behaviour interface.
    /// </summary>
    /// <param name="targetPos"></param>
    /// <param name="howClose"></param>
    /// <param name="onUpdate"></param>
    /// <param name="onClose"></param>
    /// <param name="onArrive"></param>
    /// <returns></returns>
    public static IEnumerator TravelInPath(Transform transform, Vector3[] targetPos,
        float howClose = 1f, Action<Vector3, float> onUpdate = null,
        Action<Vector3> onClose = null, Action onArrive = null)
    {
        int i = 0;
        float dist = 1;
        while (i < targetPos.Length)
        {
            dist = Vector3.Distance(targetPos[i], transform.position);
            if (onUpdate != null)
            onUpdate(targetPos[i], dist);
            if (dist <= howClose)
            {
                i++;
                if (onClose != null)
                onClose(targetPos[i]);// send new target pos
            }
            yield return null;
        }
        onArrive();
    }

    [System.Obsolete("remove this function")]
    public static bool DistanceTooBig(Vector3 targetPos, Vector3 transformPos)
    {
        if (Vector3.Distance(targetPos, transformPos) < 1f)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// It goes like this: scan 1 ship, assign all ships in range to it and remove them from next scans
    /// Repeat for all and return array with all ranges
    /// </summary>
    /// <param name="blastRadius">Put your explosion radius here</param>
    public static Transform GetApproximationOfGroupCount(MonoBehaviour requestedBy, int shipCount, ref float approxAverageNumOfShipsPerScan, Transform[] allShips, float blastRadius = 100)
    {
        Transform[] shipsNotScanned = new Transform[shipCount];

        bool[] ignore = new bool[shipCount];

        // use statistics to quick chose targets
        int maxCount = 0;
        Transform maxCountShip = null;
        int sum = 0;
        int count = 0;
        for (int i = 0; i < shipCount; i++)
        {
            if (ignore[i])
            {
                continue;
            }
            Transform unit = shipsNotScanned[i];
            if (unit == null)
            {
                continue;
            }
            //print(unit);
            //Debug.DrawRay(unit.position, Vector2.up * blastRadius, Color.cyan, 5);
            Collider2D[] results = Physics2D.OverlapCircleAll(unit.position, blastRadius, 1 << LayerMask.NameToLayer("Detection"));
            int unitCount = results.Length;
            //print(allShips.Count + " "+requestedBy + " " + results.Length + " " + unitCount);

            count++;
            sum += unitCount;

            for (int j = 0; j < results.Length; j++)
            {
                int index = IndexOf(results[j].transform.root, allShips);
                if (index > -1 && !ignore[index])
                {
                    //print(requestedBy + " " + results[j] + " " + results[j].transform.root);
                    ignore[index] = true;
                }
            }

            if (unitCount > maxCount)
            {
                maxCount = unitCount;
                maxCountShip = unit;
            }

            // early exit if optimal group is found. this will lead to towers shooting to the group 1 or 2 times before avg will increase and search will broaden
            if (approxAverageNumOfShipsPerScan > 0 && unitCount / 2 > approxAverageNumOfShipsPerScan)
            {
                break;
            }
        }

        if (maxCountShip == null)
        {
            return null;
        }

        approxAverageNumOfShipsPerScan = sum / count;
        return maxCountShip;
    }

    public static int IndexOf(Transform root, Transform[] allShips)
    {
        for (int i = 0; i < allShips.Length; i++)
        {
            if (root == allShips[i].transform)
            {
                return i;
            }
        }
        return -1;
    }

    internal static void GetDistanceSorted(Vector3 calcFrom, List<Alliance> allShips, ref TargetsDistanceSortedList<Component> sortList) {
        for (int i = 0; i < allShips.Count; i++) {
            if (allShips[i] == null) { UnitsInScene.RequestNullUpdate(); continue; }
            Transform ship = allShips[i].transform;

            float dist = Mathf.Pow(ship.position.x - calcFrom.x, 2) + Mathf.Pow(ship.position.x - calcFrom.y, 2);
            sortList.Set(ship, dist);
        }
    }

    internal static bool ContainsTransform(Transform[] sortList, Transform ship) {
        return false;
    }


    internal static List<T> GetDistanceSorted<T>(Vector3 calcFrom, ref List<T> allShips) where T : MonoBehaviour {
        List<TargetDist<T>> sorted = GetDistanceSorted<T>(calcFrom, allShips);
        List<T> sortedDist = new List<T>();
        for (int i = 0; i < sorted.Count; i++) {
            sortedDist.Add(sorted[i].target);
        }
        return sortedDist;
    }

    internal static List<TargetDist<T>> GetDistanceSorted<T>(Vector3 calcFrom, List<T> allShips) where T: MonoBehaviour {
        List<TargetDist<T>> sortedDist = new List<TargetDist<T>>();

        for (int i = 0; i < allShips.Count; i ++) {
            Transform ship = allShips[i].transform;
            if (ship == null) { UnitsInScene.RequestNullUpdate(); continue; }

            float dist = Mathf.Pow(ship.position.x - calcFrom.x, 2) + Mathf.Pow(ship.position.x- calcFrom.y, 2);

            // insertion sort 
            int j = i;
            while (j > 0 && sortedDist[j-1].dist > dist) {
                j--;// you can put like /2 or smth here to speed it up into binary log n. but it will be innacurate
            }
            sortedDist.Insert(j, new TargetDist<T>(allShips[i], dist));
        }
        return sortedDist;
    }

    internal static Transform GetClosestShip(MonoBehaviour requestBy, int optimizationSteps, Transform[] allShips)
    {
        Transform min = null;
        float mind = float.MaxValue;
        for (int i = 0; i < allShips.Length; i += optimizationSteps) {
            Transform ship = allShips[i];
            if (ship == null) {
                continue;
            }
            float dist = Mathf.Pow(ship.position.x - requestBy.transform.position.x, 2) + Mathf.Pow(ship.position.x - requestBy.transform.position.y, 2);
            if (dist < mind) {
                min = allShips[i];
                mind = dist;
            }
        }
        return min;
    }

}

class TargetsDistanceSortedList<Component> : List<TargetDist<Component>> where Component: UnityEngine.Component{
    public bool Contains(Transform obj) {
        for (int i = 0; i < Count; i++) {
            if (this[i].target.transform == obj) {
                return true;
            }
        }
        return false;
    }

    internal void Add(Transform ship, float dist) {
        base.Add(new TargetDist<Component>(ship as Component, dist));
    }

    internal void Set(Transform key, float value) {
        for (int i = 0; i < Count; i++) {
            if (this[i].target == key) {
                this[i].dist = value;
                return;
            }
        }
        // not found
        Add(key, value);
    }
}

class TargetDist<T> where T:Component {
    public T target;
    public float dist;

    public TargetDist(T target, float dist) {
        this.target = target;
        this.dist = dist;
    }
}