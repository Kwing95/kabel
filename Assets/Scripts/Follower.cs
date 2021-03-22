using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{

    public GridMover subject;
    public Vector3 offset = new Vector3(0, 0, -10);
    public float panSpeed = 3;
    public float closeEnough = 1;

    private bool paused = false;

    // Start is called before the first frame update
    protected void Start()
    {
        transform.position = subject.transform.position + offset;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(subject.GetCanTurn() && Vector2.Distance(transform.position, subject.transform.position + offset) <= closeEnough)
            paused = true;

        if (paused && !subject.GetCanTurn())
            paused = false;

        if(!paused)
            Lerp();
    }

    protected virtual void Lerp()
    {
        transform.position = Vector3.Lerp(transform.position, subject.transform.position + offset, panSpeed * Time.deltaTime);
    }

}
