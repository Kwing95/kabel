using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GassableEnemy : Gassable
{

    public GameObject playerViewMask;
    public float blindedViewDistance = 0.5f;
    private float baseViewDistance;
    private AutoMover autoMover;

    // Start is called before the first frame update
    void Start()
    {
        autoMover = GetComponent<AutoMover>();
        baseViewDistance = autoMover.sightDistance;
    }

    public override void SetGassed(bool isGassed)
    {
        base.SetGassed(isGassed);

        autoMover.sightDistance = baseViewDistance * (isGassed ? blindedViewDistance : 1);

        if (isGassed)
        {
            
        }
    }

}
