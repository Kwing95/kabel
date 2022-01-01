using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanZoom : MonoBehaviour
{
    Vector3 touchStart;
    public float zoomOutMin = 5;
    public float zoomOutMax = 14;
    public float scrollZoomSpeed = 6;
    public float maxPan = 20;
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
            if (canDrag)
            {
                if(Vector2.Distance(transform.position, PlayerMover.instance.transform.position) > maxPan)
                {
                    canDrag = false;
                    Vector2 offset = Vector3.Normalize((Vector2)transform.position - (Vector2)PlayerMover.instance.transform.position);
                    Camera.main.transform.position = new Vector3(0, 0, transform.position.z) + PlayerMover.instance.transform.position + (Vector3)(0.95f * maxPan * offset);
                }
                else
                {
                    Camera.main.transform.position += direction;
                }
            }
                
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