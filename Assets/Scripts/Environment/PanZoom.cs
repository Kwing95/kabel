using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanZoom : MonoBehaviour
{
    Vector3 touchStart;
    public float zoomOutMin = 5;
    public float zoomOutMax = 14;
    public float scrollZoomSpeed = 6;
    private bool canDrag = true;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            canDrag = !ClickManager.MouseOverUI() && !Sidebar.GetMenuPaused();
            touchStart = ClickManager.GetMousePosition(true); // Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            Zoom(difference * 0.01f);
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 direction = touchStart - (Vector3)ClickManager.GetMousePosition(true); // Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if(canDrag)
                Camera.main.transform.position += direction;
        }

        if(!Sidebar.GetMenuPaused())
            Zoom(Input.GetAxis("Mouse ScrollWheel") * scrollZoomSpeed);
    }

    void Zoom(float increment)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomOutMin, zoomOutMax);
        Camera.main.GetComponent<Zoomer>().UpdateFieldOfView();
    }


}