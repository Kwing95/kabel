using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulser : MonoBehaviour
{

    public Color c1;
    public Color c2;

    public float minSize = 1;
    public float maxSize = 1.2f;
    public float periodLength = 1;

    private SpriteRenderer sr;

    private float growthRate;
    private float p = 0;

    // Start is called before the first frame update
    void Start()
    {
        growthRate = Time.deltaTime / periodLength;
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate progress and progress_inverse
        p += growthRate;
        float p_i = 1 - p;

        sr.color = new Color(c1.r * p + c2.r * p_i, c1.g * p + c2.g * p_i, c1.b * p + c2.b * p_i);

        transform.localScale = Vector2.one * (minSize * p + maxSize * p_i);

        if (p > 1 || p < 0)
            growthRate *= -1;
    }
}
