using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[ExecuteInEditMode]
[@RequireComponent(typeof(BoxCollider2D))]
public class WaypointPlacer : MonoBehaviour {

    public BoxCollider2D ground;
    public Transform startingWaypointPrefab;
    public Transform waypointPrefab;
    public Transform endingWaypointPrefab;
    public string namingPrefix = "wp";

    List<Waypoint> spawnedWaypoints = new List<Waypoint>();

    Waypoint lastWp;
    private bool endPointDown;
    int wpSelectCount;

	// click ground: add new wp, and connect it to last if there is last
    // click wp: set it as last
    // click 

    void Update() {
        PlaceWps(null);
    }

    private void PlaceWps(SceneView sceneView) {
        /*Event e = Event.current;

        if (e.type == EventType.MouseDown && e.button == 1)
            Debug.Log("RMB was pressed");
        */

        // left mouse click spawns new waypoint or selects one. new waypoint will be connected to selected, or last spawned
        if (Input.GetKeyDown(KeyCode.Mouse0)) {

            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.transform == ground.transform && !endPointDown) {
                wpSelectCount = 0;
                SpawnNewWaypoint(hit);
            } else if (hit.transform.tag == "Waypoint") {
                // clicking 2 waypoints will connect them
                wpSelectCount ++;
                if (wpSelectCount == 2) {
                    ConnectToLastWP(hit.transform.GetComponent<Waypoint>());
                    wpSelectCount = 0;
                }
             
                SaveClicked(hit);
            }
        }
        // right mouse click sets end waypoint or deletes any waypoint or repositions end waypoint
        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.transform == ground.transform && !endPointDown && spawnedWaypoints.Count > 0) {
                wpSelectCount = 0;
                Waypoint endwp = SpawnWaypoint(endingWaypointPrefab, hit);
                endwp.name = "end" + namingPrefix + "(Clone)";
                InitWp(endwp);
                endPointDown = true;
            } else if (hit.transform.tag == "Waypoint") {
                if (wpSelectCount == 2) {
                    // remove connection between last and current
                    lastWp.RemoveConnection(hit.transform.GetComponent<Waypoint>());
                    wpSelectCount = 0;
                }// dont delte start if there are any others waypoints
                else if (spawnedWaypoints.Count == 1 || (hit.transform != spawnedWaypoints[0].transform && spawnedWaypoints.Count > 1)) {
                    if (hit.transform ==spawnedWaypoints[spawnedWaypoints.Count-1].transform ) {
                        endPointDown = false;
                        wpSelectCount = 0;
                    }
                    DeleteClicked(hit);

                }
            } else if(endPointDown){
                // if endopint is down, just reposition it. you can also delete it, but thats the other if
                wpSelectCount = 0;
                Waypoint endWp = spawnedWaypoints[spawnedWaypoints.Count - 1];
                endWp.transform.position = hit.point;
            }
        }
    }

    private void DeleteClicked(RaycastHit2D hit) {
        // if hit wp is end, unlock it
        if (hit.transform == spawnedWaypoints[spawnedWaypoints.Count-1]) {
            endPointDown = false;
        }
        spawnedWaypoints.Remove(hit.transform.GetComponent<Waypoint>());
        Destroy(hit.transform.gameObject);
    }

    void Start() {
        ground = GetComponent<BoxCollider2D>();
    }


    private void SaveClicked(RaycastHit2D hit) {
        lastWp = hit.transform.GetComponent<Waypoint>();
    }

    private void SpawnNewWaypoint(RaycastHit2D hit) {
        Waypoint wp;
        if (spawnedWaypoints.Count == 0) {
            wp = SpawnWaypoint(startingWaypointPrefab, hit);
            wp.name = "start" + namingPrefix + "(Clone)";
        }
        else { 
            wp = SpawnWaypoint(waypointPrefab, hit);
            wp.name = namingPrefix + "(Clone)";
        }
        InitWp(wp);
    }

    private void InitWp(Waypoint wp) {
        if (lastWp) {
            ConnectToLastWP(wp);
        }

        spawnedWaypoints.Add(wp);
        wp.transform.parent = transform;

        lastWp = wp;
    }

    private Waypoint SpawnWaypoint(Transform pref, RaycastHit2D hit) {
        Vector3 point = hit.point;
        point.z = 0;
        Waypoint wp = (Instantiate(pref, point, new Quaternion()) as Transform).GetComponent<Waypoint>();
        wp.transform.localScale = new Vector3(1, 1, 1);
        return wp;
    }

    private void ConnectToLastWP(Waypoint wp) {
        lastWp.AddWp(wp);
    }
}
