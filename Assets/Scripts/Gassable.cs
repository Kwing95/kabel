using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gassable : MonoBehaviour
{

    public float gasDuration = 10;
    private float gasCounter = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    protected void Update()
    {
        if (gasCounter > 0)
        {
            gasCounter -= Time.deltaTime;
            if (gasCounter < 0)
                SetGassed(false);

        }
    }

    public virtual void SetGassed(bool isGassed)
    {
        gasCounter = isGassed ? gasDuration : 0;
    }

    public bool GetGassed()
    {
        return gasCounter > 0;
    }

}
