﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoomer : MonoBehaviour
{
    public List<float> zooms;
    public float snapThreshold = 0.1f;

    public float destinationZoom = 8;
    public float zoomSpeed = 5;

    private bool paused = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!paused && Mathf.Abs(destinationZoom - Camera.main.orthographicSize) < snapThreshold)
        {
            Camera.main.orthographicSize = destinationZoom;
            UpdateFieldOfView();
            paused = true;
        }

        if (!paused)
        {
            Camera.main.orthographicSize = Camera.main.orthographicSize + ((destinationZoom - Camera.main.orthographicSize) / zoomSpeed);
            UpdateFieldOfView();
        }
    }

    public void SetDestination(float newDestination)
    {
        destinationZoom = newDestination;
        paused = false;
    }

    public void SetPause(bool value)
    {
        paused = value;
    }

    public void ZoomIn()
    {
        for (int i = zooms.Count - 1; i >= 0; --i) { 
            if (zooms[i] < Camera.main.orthographicSize && Camera.main.orthographicSize - zooms[i] > snapThreshold)
            {
                SetDestination(zooms[i]);
                break;
            }
        }
    }

    public void ZoomOut()
    {
        for (int i = 0; i < zooms.Count; ++i)
        {
            if (zooms[i] > Camera.main.orthographicSize && zooms[i] - Camera.main.orthographicSize > snapThreshold)
            {
                SetDestination(zooms[i]);
                break;
            }
        }
    }

    public void UpdateFieldOfView()
    {
        Camera.main.fieldOfView = 40 + (Camera.main.orthographicSize * 3);
    }

}
