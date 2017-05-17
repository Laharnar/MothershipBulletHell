using UnityEngine;
using System.Collections;

[@RequireComponent(typeof(BoxCollider2D))]
public class BuildLocationPlacer : MonoBehaviour {

    public Transform locationPrefab;


    void Update() {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (!hit) {
                return;
            }

            Vector3 point = hit.point;
            point.z = 0;

            Transform t = Instantiate(locationPrefab, point, new Quaternion()) as Transform;
            t.parent = transform;
        }
        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.transform.GetComponent<BuildLocation>()) {

                Destroy(hit.transform.gameObject);
            }
        }

    }
}
