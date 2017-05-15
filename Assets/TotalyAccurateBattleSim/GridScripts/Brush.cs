using UnityEngine;
using System.Collections;
using System;

public class Brush : MonoBehaviour {

    public string lookForObject = "grid";
    public Transform validObjectsParent;
    Transform[] children;

    public int activeColor = 0;
    public ColorType[] colors = new ColorType[1] { new ColorType(new Color(20, 115, 75), "unknown") };

    [System.Serializable]
    public class ColorType
    {
        public Color color;
        public string type;

        public ColorType(Color color, string type)
        {
            this.color = color;
            this.type = type;
        }
    }

	// Use this for initialization
	void Start ()
    {
        if (!validObjectsParent)
        {
            validObjectsParent = GameObject.Find(lookForObject).transform;
        }
        children = validObjectsParent.GetChildrenWithoutParent();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            RecolorItem();
        }
    }

    private void RecolorItem()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null)
        {
            Recolor2d(hit.transform);
        }
    }

    private void Recolor2d(Transform hit)
    {
        hit.GetComponent<SpriteRenderer>().color = colors[activeColor].color;
        hit.GetComponent<Alliance>().alliance = colors[activeColor].type;
    }

    public void SetActiveColor(int activeColor)
    {
        this.activeColor = activeColor;
    }
}
