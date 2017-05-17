using UnityEngine;
using System.Collections;

public class HighlightOnHover : MonoBehaviour {

    public Transform hoverHighlight;
    internal bool hovering = false;

    public bool used = true;

    //public Transform selectedHighlight;
    //internal bool selected = false;

    void Start() {
        //StartMenu();
    }

    // Update is called once per frame
    void OnMouseOver() {
        hovering = true;
        hoverHighlight.gameObject.SetActive(true);
    }


    void OnMouseDown() {
        //hoverHighlight.gameObject.SetActive(false);
        //selected = true;
        //selectedHighlight.gameObject.SetActive(true);
    }

    void Update() {
        if (!used)
        {
            return;
        }

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.transform != transform) {
            hovering = false;

        }
        //if (Input.GetMouseButtonDown(0) && hit.transform != transform) {
        //    selected = false;
        //}
      
        if (!hovering) {
            hoverHighlight.gameObject.SetActive (false);
        }
        //if (!selected) {
        //    selectedHighlight.gameObject.SetActive (false);
        //}
        //UpdateMenu();
    }


    public Transform menu;

    //void StartMenu() {
        //menu.gameObject.SetActive(selected);

    //}

    // Update is called once per frame
    //void UpdateMenu() {
        //if (Input.GetMouseButtonDown(0)) {
        //    menu.gameObject.SetActive(selected);
        //}
    //}
}
