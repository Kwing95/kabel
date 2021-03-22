using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorIndicator : MonoBehaviour
{

    public List<Color> colors;
    private SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        if (colors.Count > 0)
            sr.color = colors[colors.Count - 1];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetColor(int index)
    {
        if(index > 0 && index < colors.Count)
            sr.color = colors[index];
    }
        

}
