using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{

    public GridMover subject;
    public Vector3 offset = new Vector3(0, 0, -10);
    public float panSpeed = 3;

    // Start is called before the first frame update
    protected void Start()
    {
        transform.position = subject.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // transform.position = transform.position + ((subject.transform.position + offset - transform.position) / panSpeed);
        Lerp();
    }

    protected virtual void Lerp()
    {
        transform.position = Vector3.Lerp(transform.position, subject.transform.position + offset, panSpeed * Time.deltaTime);
    }

}
