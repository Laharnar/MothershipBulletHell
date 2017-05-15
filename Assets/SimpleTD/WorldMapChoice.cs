using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Use for showing level details after clicking the level
/// </summary>
[RequireComponent(typeof(Waypoint))]
public class WorldMapChoice : MonoBehaviour {

    Waypoint pathTarget;
    public Transform menu;
    public SpriteRenderer showLocation;// level description, enter level, etc
    public BoxCollider2D clickDetection;

    public bool levelUnlocked = true;

    static WorldMapChoice lastActive;
    public bool includeInWorld = true;

    // Use this for initialization
    void Awake() {
        pathTarget = GetComponent<Waypoint>();
        pathTarget.additionalTarget = this;

        showLocation = GetComponent<SpriteRenderer>();
        clickDetection = GetComponent<BoxCollider2D>();
    }
    void Start() { 
        if (pathTarget.GetType() == typeof(StartingWaypoint))
        {
            DisplayUnlockedPaths();
        }
        menu.gameObject.SetActive(false);

        Close();
    }

    private void Close()
    {
        clickDetection.enabled = (false);
        showLocation.enabled = (false);

        if (lastActive == this)
        {
            lastActive = null;
        }
    }

    void Update()
    {
        if (levelUnlocked)
            this.TryOpen();
    }

    private void DisplayUnlockedPaths()
    {
        for (int i = 0; i < pathTarget.subpoints.Count; i++)
        {
            Waypoint wp = pathTarget.subpoints[i];
            if (wp)
            {
                ((WorldMapChoice)wp.additionalTarget).TryOpen();
            }
        }
    }

    // Update is called once per frame
    void OnMouseDown ()
    {
        Toggle();
    }

    private void Toggle()
    {
        menu.gameObject.SetActive(!menu.gameObject.activeSelf);
        // if toggle is linked to scene toggle, it will set it as last
        if (menu.gameObject.activeSelf && includeInWorld)
        {
            lastActive = this;
        }
    }
}

public static partial class Helper
{
    public static void TryOpen(this WorldMapChoice openMenu)
    {
        // open menu for this level
        openMenu.clickDetection.enabled = (openMenu.levelUnlocked);
        openMenu.showLocation.enabled = (openMenu.levelUnlocked);
    }
}