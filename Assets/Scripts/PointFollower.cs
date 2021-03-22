using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointFollower : MonoBehaviour
{

    public Vector2 target;
    public float panSpeed = 1;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target, panSpeed * Time.deltaTime);
    }
}
