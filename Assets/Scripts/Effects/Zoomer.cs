using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoomer : MonoBehaviour
{

    public float destinationZoom = 8;
    public float zoomSpeed = 5;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Camera.main.orthographicSize = Camera.main.orthographicSize + ((destinationZoom - Camera.main.orthographicSize) / zoomSpeed);
    }
}
