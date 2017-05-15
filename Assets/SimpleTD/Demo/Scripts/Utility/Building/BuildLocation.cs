using UnityEngine;
using System.Collections;


public class BuildLocation : MonoBehaviour {

  /*  public OutOfScreenObject menu;
    public OutOfScreenObject highlight;
    public OutOfScreenObject onSelectedHighlight;


	// Use this for initialization
	void Start () {
        menu.SavePos();
        highlight.SavePos();
        onSelectedHighlight.SavePos();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (!hit.transform.GetComponent<BuildLocation>()) {
                TowerBuildManager.Deselect(this);
                menu.Revert();
                highlight.Revert();
                onSelectedHighlight.Revert();
            }
        }
    }

    void OnMouseOver() {
        if (!highlight.visible) {
            highlight.Open(transform.position);
        }
    }

    void OnMouseExit() {
        highlight.Revert();
    }

    void OnMouseDown(){
        if (!menu.visible) {
            TowerBuildManager.Select(this);
            menu.Open(transform.position);
            onSelectedHighlight.Open(transform.position);
        } else {
            menu.Revert();
            onSelectedHighlight.Revert();
        }
        
    }
    */
}

/// <summary>
/// for hud, not for buttons
/// </summary>
[System.Serializable]
public class OutOfScreenObject {
    public Transform target;
    internal Vector3 outOfScreenPos;
    internal bool visible;

    /// <summary>
    /// save targets pos into last pos.
    /// Careful when you use this
    /// </summary>
    public void SavePos() {
        outOfScreenPos = target.position;
    }

    public void Open(Vector3 where) {
        target.position = where;
        visible = true;
    }

    public void Revert() {
        target.position = outOfScreenPos;
        visible = false;
    }


    internal void SavePos(Transform customOutOfScreenPos) {
        outOfScreenPos = customOutOfScreenPos.transform.position;
    }
}