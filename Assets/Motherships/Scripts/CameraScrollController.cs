using UnityEngine;
using System.Collections;

/// <summary>
/// rts style camera scrolling with wasd keys, zooming, mouse moving and resetting
/// </summary>
public class CameraScrollController : MonoBehaviour {

    public bool
        useReset = true,
        useMouse = true,
        useKeyboard = true,
        useZoom = true;
    
    
    public Transform resetPos;
    Vector3 originalPos;

    /// <summary>
    /// Tells width of area, near edges of screen, in which scrolling starts to work
    /// ex: 5, 5, 5, 5, scrolling is 5px wide edge on all sides
    /// </summary>
    public RectOffset scrollMargin;
    public float scrollSensitivity= 5;// scroll sensitivity multiplier

    new Camera camera;
    float originalZoom;
    float zoom;
    public float zoomSensitivity = 1;

    void Start() {
        // init camera
        camera = GetComponent<Camera>();
        originalPos = camera.transform.position;

        // init zoom
        originalZoom = camera.fieldOfView;
        zoom = originalZoom;
    }

	// Update is called once per frame
	void Update () {
        float fov = camera.orthographicSize;// field of view, for keeping movement faster if its zoomed out

        // move cam around
        float camSpeed = scrollSensitivity * fov *Time.deltaTime;
        if (useMouse) {
            float mouseX = Input.mousePosition.x;
            float mouseY = Input.mousePosition.y;

            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            if (mouseX <= scrollMargin.left) {
                camera.transform.Translate(Vector2.left * camSpeed);
            }
            if (mouseX >= screenWidth - scrollMargin.right) {
                camera.transform.Translate(Vector2.right * camSpeed);
            }
            if (mouseY <= scrollMargin.bottom) {
                camera.transform.Translate(Vector2.down * camSpeed);
            }
            if (mouseY >= screenHeight - scrollMargin.top) {
                camera.transform.Translate( Vector2.up * camSpeed);
            }
        }

        // camera movement with keyboard 
        if (useKeyboard) {
            camera.transform.Translate(Input.GetAxis("Horizontal") * camSpeed, Input.GetAxis("Vertical") * camSpeed, 0);
        }

        // key resets position to object
        if (useReset) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                camera.transform.position = originalPos + resetPos.position;
            }         
        }

        if (useZoom) {
            
            float middleMouse = Input.GetAxis("Mouse ScrollWheel");
            camera.orthographicSize = camera.orthographicSize - middleMouse*zoomSensitivity;
        }

	}
}
