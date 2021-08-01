using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{

    private bool reached = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!reached && Vector2.Distance(transform.position, PlayerMover.instance.transform.position) < 0.5f)
        {
            TransitionFader.instance.FinishLevel();
            reached = true;
        }
    }
}
