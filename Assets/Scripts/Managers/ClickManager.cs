using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClickManager : MonoBehaviour
{
    public delegate void ClickHandler(Vector2 position);
    public static ClickHandler pressHandler;
    public static ClickHandler releaseHandler;

    private static float minDragLength = 1;
    private static bool isMobile = false;
    // private static bool mouseDown = false;
    private static bool initialClick = true;
    private static bool uiClick = false;
    private static bool hasDragged = false;
    private Vector2 initialPosition;

    private static float enter = 0.0f;
    private static Plane gamePlane = new Plane(Vector3.forward, Vector3.zero);
    private static Ray ray;

    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log("instance of ClickManager running on " + gameObject.name);
        DetectPlatform();
    }

    // Update is called once per frame
    void Update()
    {
        // On press
        if (Input.GetMouseButton(0))
        {
            if (initialClick)
            {
                uiClick = MouseOverUI();
                initialClick = false;
                initialPosition = Camera.main.transform.position;
                initialClick = false;
                hasDragged = false;

                if (uiClick)
                    return;

                pressHandler?.Invoke(GetMousePosition());

                /**
                // Testing enemy response to noise
                GameObject tempNoise = Instantiate(Globals.NOISE, transform.position, Quaternion.identity);
                tempNoise.transform.position = GetMousePosition();
                tempNoise.GetComponent<Noise>().Initialize(true, 3.5f);
                /**/
            }
            if(Vector2.Distance(initialPosition, Camera.main.transform.position) >= minDragLength)
            {
                hasDragged = true;
            }
        }

        if (!Input.GetMouseButton(0))
        {
            if (!initialClick && !hasDragged && !uiClick)
            {
                releaseHandler?.Invoke(GetMousePosition());
            }
            
            initialClick = true;
        }
    }

    public static Vector2 GetMousePosition(bool floating=false)
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (gamePlane.Raycast(ray, out enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            return floating ? hitPoint : Vector3Int.RoundToInt(hitPoint);
        }

        Debug.Log("Click did not intersect with gamePlane");
        return Vector2.zero;

        // return (Vector3)Vector3Int.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    public static void DetectPlatform()
    {
        isMobile = Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer;
    }

    // Returns true if mouse is down
    public static bool GetMouseDown()
    {
        if (isMobile)
            return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended;
        else
            return Input.GetKeyDown(KeyCode.Mouse0);
    }

    // Returns true if clicked world and not UI element
    public static bool GetValidMouseDown()
    {
        if (isMobile)
            return GetMouseDown() && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        else
            return GetMouseDown() && !EventSystem.current.IsPointerOverGameObject();
    }

    public static bool MouseOverUI()
    {
        if (isMobile)
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        else
            return EventSystem.current.IsPointerOverGameObject();
    }

    public static bool IsDrag(Vector2 pressPosition, Vector2 releasePosition)
    {
        return Vector2.Distance(pressPosition, releasePosition) > minDragLength;
    }

}
