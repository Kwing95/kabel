using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessyFollower : Follower
{
    public float cutoffDistance = 0.4f;

    private bool moving = false;

    // Update is called once per frame
    void LateUpdate()
    {
        Lerp();
    }

    protected override void Lerp()
    {
        if (Vector2.Distance(transform.position, subject.transform.position) >= cutoffDistance)
        {
            base.Lerp();
            moving = true;
        }
        else
            moving = false;
    }

    public bool GetMoving()
    {
        return moving;
    }

}
