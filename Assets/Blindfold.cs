using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blindfold : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        SetSightDistance(11.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSightDistance(float distance)
    {
        transform.localScale = (0.5f + distance) * Vector2.one;
    }

}
