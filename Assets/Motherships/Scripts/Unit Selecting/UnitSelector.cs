using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// This script selects all units on layer "SelectionLayer".
/// 
/// Script allows single click and area select.
/// 
/// </summary>
// Example setup --
// 1) Put this script on 1 object.
// 2) Add objects with colliders on layer 'SelectionLayer'. Colliders can be trigger or normal as long as they can't interact with anything
// 3) Add object with Image component, set its anchor to upper left, and witdh and height to 0
// 4) Test it
public class UnitSelector : MonoBehaviour {
    private Vector2 mouseClickStart;
    private Vector2 mouseClickEnd;
    string layerMask = "SelectionLayer";

    List<MonoBehaviour> selected;

    public bool areUnitsSelected { get { return selected.Count > 0; } }
    public int SelectionSize { get { return selected.Count; } }

    //Gun[] gunComponents;
    //UpgradePlatform[] upgradePlatforms;


    /// <summary>
    /// Image used to display selection area.
    /// 
    /// Image should be anchored to upper left corner, with width and height 0
    /// </summary>
    public Image selectAreaImage;

    /// <summary>
    /// Changes to true every time selection changes or clears
    /// </summary>
    bool _selectionChanged;
    internal bool selectionChanged {
        get {
            return _selectionChanged;
        }
        private set {
            SetIsUpdated<UpgradePlatform>("UpgradePlatforms", false);
            SetIsUpdated<Gun>("GunComponents", false);
            SetIsUpdated<LaserGun>("LaserComponents", false);
            _selectionChanged =value;
        }
    }

    interface ISelectionArray {
        
    }

    class SelectionArray<T> : ISelectionArray {
        internal T[] accessComponents;
        internal bool updated;

        string comment;

        public SelectionArray(string comment) {
            this.comment = comment;
        }
    }

    //SelectionArray<Gun> gunComponents;
    //SelectionArray<UpgradePlatform> upgradePlatforms;

    

    Dictionary<string, ISelectionArray> selectionArrays = new Dictionary<string, ISelectionArray>()
    {
        { "GunComponents", new SelectionArray<Gun>("GunComponents") },
        { "UpgradePlatforms", new SelectionArray<UpgradePlatform>("UpgradePlatforms") },
        { "LaserComponents", new SelectionArray<LaserGun>("LaserComponents") }
    };


	// Use this for initialization
	void Start () {
        selected = new List<MonoBehaviour>();

	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0)) {
            StartCoroutine(SelectArea());
        }
	}

    IEnumerator SelectArea() {
        mouseClickStart = Input.mousePosition;
        float time = Time.time+0.15f;
        // wait for some time before starting area selection in case user just wanted to use normal point selection
        do {
            // early mouse up, cancel area click
            if (Input.GetMouseButtonUp(0)) {
                SelectPoint();
                yield break;
            }
            yield return null;
        } while (Time.time < time);
        
        // area selecting, wait for mosue release
        print("Area selecting mode - layer = 'SelectionLayer'");
        while (!Input.GetMouseButtonUp(0))
	    {
            selectAreaImage.rectTransform.position = new Vector2(mouseClickStart.x, mouseClickStart.y);//left/top        
            selectAreaImage.rectTransform.sizeDelta = new Vector2(Input.mousePosition.x - mouseClickStart.x, -Input.mousePosition.y + mouseClickStart.y); // right/bottom
	        yield return null;
	    }

        //get units in area
        mouseClickEnd = Input.mousePosition;
        GetUnitsFromArea(mouseClickStart, mouseClickEnd, layerMask);

        //clear selection area gui
        selectAreaImage.rectTransform.position = new Vector2(0, 0);//left/top
        selectAreaImage.rectTransform.sizeDelta = new Vector2(0, 0); // right/bottom
	        
    }

    private void GetUnitsFromArea(Vector2 mouse1, Vector2 mouse2, string layerMask) {
        // get units in area and saves them to selected. units have to be on layer "SelectionLayer"
        Collider2D[] area = Physics2D.OverlapAreaAll(
            Camera.main.ScreenToWorldPoint(mouse1),
            Camera.main.ScreenToWorldPoint(mouse2), 1 << LayerMask.NameToLayer(layerMask));

        // reset
        if (area.Length > 0) {
            selected.Clear();
        }
        selectionChanged = true;
        // store selected
        for (int i = 0; i < area.Length; i++) {
            selected.Add(area[i].GetComponent<SelectableObject>().target);

            Debug.Log("Area selection/Result:" + area[i].GetComponent<SelectableObject>().target.transform.name + "(Child contant:" + area[i].transform.name + ")", this);
        }
    }

    /// <summary>
    /// Attempt to get object under mouse
    /// </summary>
    void SelectPoint() {

        if(selected.Count >0) {
            selectionChanged = true;
        }
        selected.Clear();
        RaycastHit2D hit2d = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, 1 << LayerMask.NameToLayer(layerMask));
        if (hit2d.transform) {
            selected.Add(hit2d.collider.GetComponent<SelectableObject>().target);
            selectionChanged = true;
            print("Point selecting - layer = '" + layerMask + "' Result:" + hit2d.collider.GetComponent<SelectableObject>().target.transform.name + "(Child contant:" + hit2d.collider.transform.name + ")");
        }
    }

    #region Helper functions
    T[] GetUpdatedSelection<T>(string key) {
        return ((SelectionArray<T>)(selectionArrays[key])).accessComponents;
    }

    void SetUpdatedSelection<T>(string key, T[] value) {
        ((SelectionArray<T>)(selectionArrays[key])).accessComponents = value;
    }

    bool GetIsUpdated<T>(string key) {
        return ((SelectionArray<T>)(selectionArrays[key])).updated;
    }

    void SetIsUpdated<T>(string key, bool value) {
        ((SelectionArray<T>)(selectionArrays[key])).updated = value;
    }

    /// <summary>
    /// Gets all components of selected guns. Components are collected once per selection, and only if gun components were needed by other scripts.
    /// </summary>
    /// <returns></returns>
    internal Gun[] GetGuns {
        get {
            Gun[] gs = GetComponentsInSelected<Gun>();
            if (!GetIsUpdated<Gun>("GunComponents")) {
                SetUpdatedSelection<Gun>("GunComponents", gs);
                SetIsUpdated<Gun>("GunComponents", true);
            }
            return gs;
        }
    }

    /// <summary>
    /// Gets all components of selected guns. Components are collected once per selection, and only if gun components were needed by other scripts.
    /// </summary>
    /// <returns></returns>
    internal LaserGun[] GetLasers {
        get {
            LaserGun[] gs = GetComponentsInSelected<LaserGun>();
            if (!GetIsUpdated<LaserGun>("LaserComponents")) {
                SetUpdatedSelection<LaserGun>("LaserComponents", gs);
                SetIsUpdated<LaserGun>("LaserComponents", true);
            }
            return gs;
        }
    }

    internal UpgradePlatform[] GetUpgradePlatforms {
        get {
            UpgradePlatform[] gs = GetComponentsInSelected<UpgradePlatform>();
            if (!GetIsUpdated<UpgradePlatform>("UpgradePlatforms")) {
                SetUpdatedSelection<UpgradePlatform>("UpgradePlatforms", gs);
                SetIsUpdated<UpgradePlatform>("UpgradePlatforms", true);
            }
            return gs;
        }
    } 
        /*if (!selectionChanged) {
            return this.gunComponents;
        }
        List<Gun> guns = new List<Gun>();

        for (int i = 0; i < selected.Count; i++) {
            Gun g = selected[i].GetComponent<Gun>();
            if (g) {
                guns.Add(g);
            }
        }
        this.gunComponents = guns.ToArray();
        selectionChanged = false;
        return this.gunComponents;*/

    internal T[] GetComponentsInSelected<T>() {
        List<T> list = new List<T>();
        for (int i = 0; i < selected.Count; i++) {
            if (selected[i].GetType() == typeof(T)) {
                list.Add((T)(object)selected[i]);

            }
        }
        return list.ToArray();
    }
    #endregion
}
