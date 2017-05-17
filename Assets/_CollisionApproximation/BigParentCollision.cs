using UnityEngine;
using System.Collections;
using System;

// keeps resizing to aporximated size of child units
public class BigParentCollision : MonoBehaviour {

    public BigParentCollision parentCollision;
    
    UnitInfo[] registered = new UnitInfo[10000];
    int lastRegistered = 0;

    BigParentCollision[] registeredSubcollisions = new BigParentCollision[30];// sublayer has up to 30 colliders
    int lastSubcollision = 0;

    public CircleCollider2D actualCollider;
    Rect bounds;

    Rect averageBounds;
    Rect averageOffset;
    int boundCheckCount;
    int offsetChangeCount;
    public float averageWeight = 0.95f;
    public float outsideWeight = 1; // how much are new values worth
    public float insideWeight = 0.5f; // units will ussualy be inside, so don't use that much
    public Vector2 averageLocationWeight = Vector2.zero; // units will ussualy be inside, so don't use that much
    public Vector2 averageOffsetWeight = Vector2.zero;

    float avgMinLeft = 0, avgMaxRight = 0, avgMaxUp = 0, avgMinDown = 0;
    int boundCalcCount = 0;
    public float scaling = 1;

    // Use this for initialization
    void Start () {
        if (parentCollision == null)
            parentCollision = transform.parent != null ? transform.parent.GetComponent<BigParentCollision>() : null;
        if (parentCollision!= null)parentCollision.RegisterCollisionLayer(this);

        StartCoroutine(CheckSize());
	}

    private void RegisterCollisionLayer(BigParentCollision bigParentCollision) {
        registeredSubcollisions[(lastSubcollision++)%registeredSubcollisions.Length] = bigParentCollision;
        if (lastSubcollision == 0) {
            Debug.Log("Too many objects for array with default size of "+registeredSubcollisions.Length);
        }
    }

    private IEnumerator CheckSize() {
        float t = Time.time + 0.1f;
        while (true) {
            if (Time.time > t) {
                t = Time.time + 0.1f;
                Resize();
            }
            yield return null;
        }
    }

    public void RegisterUnit(UnitInfo unit) {
        registered[(lastRegistered++) % registered.Length] = unit;
        if (lastRegistered== 0) {
            Debug.Log("Too many objects for array with default size of " + registered.Length);
        }
    }

    void Resize() {
        // v2


        if (lastRegistered == 0) {
            Debug.Log("No registered units.", this);
            return;
        }
        // calculate square bounds for 10 random units and compare them to current size.
        // then aproximate the current size to that
        float avgLocalx = 0.0f;
        float avgLocaly = 0.0f;

        float minLeft = 10000000, maxRight = -10000000, maxUp = -1000000, minDown = 10000000;
        int numOfUnitsTaken = 5;
        int[] used = new int[lastRegistered];
        string nulls = "";
        int minPool = 0, maxPool = lastRegistered;
        int infite = 3000;
        for (int i = 0; i < numOfUnitsTaken; i++) {
            // select number randomly from pool of unused items
            int index = (int)UnityEngine.Random.Range(minPool, maxPool);
            bool allUnitsUsed = false;
            if (used[index] != 0) { // slot is already used

                if (index == minPool)
                    while (used[minPool] != 0) {
                        minPool++;
                        if (--infite < 0) break;
                        if (minPool >= maxPool) {
                            allUnitsUsed = true;
                            break;
                        }// all slots were used
                    }
                if (index == maxPool - 1)
                    while (used[maxPool - 1] != 0) {
                        maxPool--;
                        if (--infite < 0) break;
                        if (maxPool <= minPool) {
                            allUnitsUsed = true;
                            break;
                        }
                    }
                i--;
                if (--infite < 0 || allUnitsUsed) break;
                continue;
            }
            if (--infite < 0) { Debug.Log("break infite"); break; }
            if (allUnitsUsed) break;
            used[index] = 1; // 1 means it's used


            UnitInfo unit = registered[index];
            if (unit == null) {
                nulls += i + " ";
                continue;
            }
            if (unit.GetCompt<CircleCollider2D>()) {
                float r = unit.GetLastCompt<CircleCollider2D>().radius;
                Vector3 pos = unit.transform.position;
                float left = pos.x - r;
                float right = pos.x + r;
                float up = pos.y + r;
                float down = pos.y - r;

                //find min left, max right, max up, min down, those are the new bounds.
                if (left < minLeft) minLeft = left;
                if (right > maxRight) maxRight = right;
                if (up > maxUp) maxUp = up;
                if (down < minDown) minDown = down;

                //                Debug.Log("l"+left + " r" + right + " u" + up + " d" + down);

            } else {
                Debug.Log("No collider on unit.");
            }
        }
  //      Debug.Log("l" + minLeft + " r" + maxRight + " u" + maxUp + " d" + minDown);
        Debug.Log(nulls);


        // calculate new average
        boundCalcCount++;
        avgMaxUp = avgMaxUp * averageWeight + (1 - averageWeight) * ((boundCalcCount - 1) * maxUp / boundCalcCount);
        avgMinDown = avgMinDown * averageWeight + (1 - averageWeight) * ((boundCalcCount - 1) * minDown / boundCalcCount);
        avgMinLeft = avgMinLeft * averageWeight + (1 - averageWeight) * ((boundCalcCount - 1) * minLeft / boundCalcCount);
        avgMaxRight = avgMaxRight * averageWeight + (1 - averageWeight) * ((boundCalcCount - 1) * maxRight / boundCalcCount);
//        Debug.Log("avg l" + avgMinLeft + " r" + avgMaxRight + " u" + avgMaxUp + " d" + avgMinDown);

        //collider's position is at the center of that.
        float width = avgMaxRight - avgMinLeft;
        float height = avgMaxUp - avgMinDown;

        Vector2 center = new Vector2(avgMinLeft + width / 2, avgMinDown + height / 2);

        float whRatio = Mathf.Min(width, height) / Mathf.Max(width, height);
        // whRatio is bigger value 2/3 = bigger, smaller = 1/3.  so first make sure it really is bigger
        if (whRatio < 0.5f)
            whRatio = (1 - whRatio);
        float radius = 1;
        if (width <= height)
            radius = (width * (1-whRatio) + height * whRatio) / 2;//radius is width/2+height/2_/2, avg of half width and half height
        else radius = ((width *whRatio + height * (1-whRatio)) / 2);

        //in the end apply calculation to collider
        actualCollider.offset = center;
        actualCollider.radius = radius * scaling;
    }
}
