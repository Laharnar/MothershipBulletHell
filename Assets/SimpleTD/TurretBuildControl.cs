using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Turret build control maanger all states on tower. 1 per turret root
// Doesn't require layer setup, just tags on scripts that use the callback
public class TurretBuildControl : MonoBehaviour {

    enum TowerType {
        blue,
        red,
        green,
        yellow
    }

    List<Transform> menus;

    public bool _interactive = true;
    public bool interactive { get { return _interactive; }
        set {
            _interactive = value;
            if (additionalSettings.controlHighlight)
            additionalSettings.controlHighlight.used = value;
        }
    }

    public AdditionalSettings additionalSettings = new AdditionalSettings();

    public class AdditionalSettings
    {
        public HighlightOnHover controlHighlight;
    }

    public Transform deselectGround;
    public Transform ground;
    public Transform groundMenu;
    public Transform highlightSelected;


    public Transform t1blueTower;
    public Transform t1yellowTower;
    public Transform t1greenTower;
    public Transform t1redTower;

    public Transform t2blueTower;
    public Transform t2yellowTower;
    public Transform t2greenTower;
    public Transform t2redTower;
    
    public Transform t3blueTower;
    public Transform t3yellowTower;
    public Transform t3greenTower;
    public Transform t3redTower;

    public Transform blueUpgradeMenu;
    public Transform yellowUpgradeMenu;
    public Transform greenUpgradeMenu;
    public Transform redUpgradeMenu;
    public Transform sellMenu;

    Transform activeUpgrade;

    List<Transform> ignored ;
    public bool tests = true;

    public string lastTag;

    /* List of tags TO BE USED IN OnMousePressedDown
     * - GROUND
     * DeselectGround
     * Ground
     * 
     * - BUILD BASIC TOWER
     * GroundMenu_blue
     * GroundMenu_red
     * GroundMenu_green
     * GroundMenu_yellow
     * 
     * - SELECT TOWERS
     * Tier1_blue
     * Tier1_red
     * Tier1_green
     * Tier1_yellow
     * 
     * Tier2_blue
     * Tier2_red
     * Tier2_green
     * Tier2_yellow
     * 
     * Tier3_blue
     * Tier3_red
     * Tier3_green
     * Tier3_yellow
     * 
     * - UPGRADES
     * LvlUpBlue 
     * LvlUpRed
     * LvlUpGreen
     * LvlUpYellow
     * 
     * - SELL
     * Sell
     * */

    // Update is called once per frame
    void Awake () {

        ignored = new List<Transform>();
        menus = new List<Transform>();

        menus.Add(ground);
        menus.Add(groundMenu);
        menus.Add(highlightSelected); 
        
        menus.Add(t1blueTower);
        menus.Add(t1yellowTower);
        menus.Add(t1greenTower);
        menus.Add(t1redTower);

        menus.Add(t2blueTower);
        menus.Add(t2yellowTower);
        menus.Add(t2greenTower);
        menus.Add(t2redTower);

        menus.Add(t3blueTower);
        menus.Add(t3yellowTower);
        menus.Add(t3greenTower);
        menus.Add(t3redTower);

        menus.Add(blueUpgradeMenu);
        menus.Add(yellowUpgradeMenu);
        menus.Add(greenUpgradeMenu);
        menus.Add(redUpgradeMenu);
        menus.Add(sellMenu);

        ignored.Add(ground);
        CloseAll(ignored);
	}

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit)
            {
                ClickableObject click = hit.collider.GetComponent<ClickableObject>();
                if (click)
                    click.OnManualMouseDown();
            }
        }
       
    }

    private IEnumerator Execute(string p, bool critical = false) {
        OnMousePressedDown(p);
        if (critical) {
            yield return new WaitForSeconds(2f);
        } else {
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    // Called by on click script from all interactible objects
    public void OnMousePressedDown(ClickableObject click) {
        if (!interactive)
        {
            return;
        }
        string tag = click.clickTag;
        OnMousePressedDown(tag);
    }

    public void OnMousePressedDown(string tag) {
        if (!interactive)
        {
            return;
        }
        lastTag = tag;
        if (tag == "DeselectGround")
            Deselect();

        else if (tag == "Ground")
            Ground();

        else if (tag == "GroundMenu_blue")
            GroundMenu(TowerType.blue);
        else if (tag == "GroundMenu_red")
            GroundMenu(TowerType.red);
        else if (tag == "GroundMenu_green")
            GroundMenu(TowerType.green);
        else if (tag == "GroundMenu_yellow")
            GroundMenu(TowerType.yellow);

        // click tier 1 turret
        else if (tag == "Tier1_blue")
            Tier1("blue");
        else if (tag == "Tier1_red")
            Tier1("red");
        else if (tag == "Tier1_green")
            Tier1("green");
        else if (tag == "Tier1_yellow")
            Tier1("yellow");

        // click tier 2 turret
        else if (tag == "Tier2_blue")
            Tier2("blue");
        else if (tag == "Tier2_red")
            Tier2("red");
        else if (tag == "Tier2_green")
            Tier2("green");
        else if (tag == "Tier2_yellow")
            Tier2("yellow");

        // click tier 3 turret
        else if (tag == "Tier3_blue")
            Tier3("blue");
        else if (tag == "Tier3_red")
            Tier3("red");
        else if (tag == "Tier3_green")
            Tier3("green");
        else if (tag == "Tier3_yellow")
            Tier3("yellow");

        // click upgrade buttons
        else if (tag == "LvlUpBlue")
            BlueUpgradePath(tag);
        else if (tag == "LvlUpRed")
            RedUgpradePath(tag);
        else if (tag == "LvlUpGreen")
            GreenUpgradePath(tag);
        else if (tag == "LvlUpYellow")
            YellowUpgradePath(tag);


        else if (tag == "Sell")
            Sell();
        else {
            print("Unknown tag;" + tag);
        }
        /*
        Locked();*/
        
    }

    private void YellowUpgradePath(string tag) {
        if (activeUpgrade == t1yellowTower) {
            BuildTower(t2yellowTower);
        }
        else if (activeUpgrade == t2yellowTower) {
            BuildTower(t3yellowTower);
        }
        else if (activeUpgrade == t3yellowTower) {
            //BuildTower(t2yellowTower);
            // not included
        }
    }

    private void GreenUpgradePath(string tag) {
        if (activeUpgrade == t1greenTower) {
            BuildTower(t2greenTower);
        }
        else if (activeUpgrade == t2greenTower) {
            BuildTower(t3greenTower);
        }
        else if (activeUpgrade == t3greenTower) {
            //BuildTower(t2greenTower);
            // not included
        }
    }

    private void BlueUpgradePath(string tag) {
        if (activeUpgrade == t1blueTower) {
            BuildTower(t2blueTower);
        }
        else if (activeUpgrade == t2blueTower) {
            BuildTower(t3blueTower);
        }
        else if (activeUpgrade == t3blueTower) {
            //BuildTower(t2blueTower);
            // not included
        }
    }

    private void BuildTower(Transform tower) {
        Show(tower);
        activeUpgrade = tower;
    }

    private void RedUgpradePath(string tag) {
        if (activeUpgrade == t1redTower) {
            BuildTower(t2redTower);
        }
        else if (activeUpgrade == t2redTower) {
            BuildTower(t3redTower);
        }
        else if (activeUpgrade == t3redTower) {
            //BuildTower(t2blueTower);
            // not included
        }
    }

    private void Sell() {
        Show(ground);
        activeUpgrade = null;
    }

    private void Deselect() {
        if (activeUpgrade) {
            Show(activeUpgrade);
        } else {
            Show(ground);
        }
    }

    private void GroundMenu(TowerType colChoice) {
        switch (colChoice) {
            case TowerType.blue:
                BuildTower(t1blueTower);
                break;
            case TowerType.red:
                BuildTower(t1redTower);
                break;
            case TowerType.green:
                BuildTower(t1greenTower);
                break;
            case TowerType.yellow:
                BuildTower(t1yellowTower);
                break;
            default:
                break;
        }
    }

    private void Show(params Transform [] towers) 
    {
        if (!interactive)
        {
            return;
        }
        ignored = new List<Transform>(towers);
        CloseAll(ignored);

        if(towers != null)
        for (int i = 0; i < towers.Length; i++) {
            towers[i].gameObject.SetActive(true);
        }
    }

    private void Tier1(string tag) {
        ShowMoreMatchTag("blue", tag, blueUpgradeMenu);
        ShowMoreMatchTag("green", tag, greenUpgradeMenu);
        ShowMoreMatchTag("red", tag, redUpgradeMenu);
        ShowMoreMatchTag("yellow", tag, yellowUpgradeMenu);
        ShowMore(highlightSelected);
    }

    private void ShowMoreMatchTag(string tag, string secondTag, Transform menu) {
        if (tag == secondTag) {
            ShowMore(menu, sellMenu);
        }
    }

    private void Tier2(string tag) {
        Tier1(tag);// tier 2 upgrade buttons are the same as level 1
    }

    private void Tier3(string tag) {
        ShowMore(sellMenu);
        ShowMore(highlightSelected);
    }

    private void ShowMore(params Transform[] ignoreMore) {
        if (!interactive)
        {
            return;
        }
        this.ignored.AddRange(ignoreMore);

        if (ignored != null)
        for (int i = 0; i < ignored.Count; i++) {
            ignored[i].gameObject.SetActive(true);
        }
    }

    private void Ground() {
        // deselect on reclicking ground
        if (groundMenu.gameObject.activeSelf) {
            Show(ground);
        }
        else ShowMore(groundMenu, highlightSelected);
    }

    private void CloseAll(List<Transform> ignoring) {
        this.ignored = ignoring;
        for (int i = 0; i < menus.Count; i++) {
            bool ignore = true;
            if (ignored != null)
            for (int j = 0; j < ignored.Count; j++) {
                if (menus[i] == ignored[j]) {
                    ignore = false;
                    break;
                }
            }
            if (ignore || ignore == null || ignored.Count == 0) {
                menus[i].gameObject.SetActive(false);
            }
        }
    }
}
