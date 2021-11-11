using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fader : MonoBehaviour
{

    public float fadeRate = 0.5f;
    private Color color;

    // Start is called before the first frame update
    void Start()
    {
        color = GetComponent<SpriteRenderer>().color;
    }

    // Update is called once per frame
    void Update()
    {
        color.a -= fadeRate * Time.deltaTime;
        GetComponent<SpriteRenderer>().color = color;

        if (color.a <= 0)
            Destroy(gameObject);
    }
}
