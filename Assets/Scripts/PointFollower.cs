using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointFollower : MonoBehaviour
{

    public Vector2 target;
    public float panSpeed = 1;
    private float timeAlive = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timeAlive += Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, target, panSpeed * Time.deltaTime);
    }

    public float GetTimeAlive()
    {
        return timeAlive;
    }
}
