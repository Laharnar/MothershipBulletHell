using UnityEngine;
using System.Collections;

[@RequireComponent(typeof(BoxCollider2D))]
public class ClickableObject : MonoBehaviour {

    public string clickTag;
    public TurretBuildControl[] onClickCallbackTarget;

    void Start() {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            foreach (var hit in hits)
            {
                if (hit.transform == transform)
                {
                    print("c");
                    OnMouseDownM();
                }
            }
        }
    }


    void OnMouseDownM()
    {
        for (int i = 0; i < onClickCallbackTarget.Length; i++)
        {
            if (onClickCallbackTarget[i])
            {
                onClickCallbackTarget[i].OnMousePressedDown(this);
            }
        }
        if (onClickCallbackTarget == null || onClickCallbackTarget.Length == 0)
        {
            print("warning, nowhere sent on click");
        }
    }

    public void OnManualMouseDown() {
        return;
        Debug.Log("clicked "+transform, transform);
        for (int i = 0; i < onClickCallbackTarget.Length; i++) {
            if (onClickCallbackTarget[i]) {
                onClickCallbackTarget[i].OnMousePressedDown(this);
            }
        }
        if (onClickCallbackTarget == null || onClickCallbackTarget.Length == 0) {
            print("warning, nowhere sent on click");
        }
    }
}

