using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GassablePlayer : Gassable
{
    public GameObject playerViewMask;
    public float blindedViewDistance = 0.5f;
    private float baseViewDistance;

    // Start is called before the first frame update
    void Start()
    {
        baseViewDistance = playerViewMask.transform.localScale.x;
    }

    public override void SetGassed(bool isGassed)
    {
        base.SetGassed(isGassed);

        float scale = baseViewDistance * (isGassed ? blindedViewDistance : 1);
        playerViewMask.transform.localScale = new Vector2(scale, scale);

        if (isGassed)
        {
            
        }
    }

}
