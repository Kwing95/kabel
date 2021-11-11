using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlphaSlider : MonoBehaviour
{

    public float fadeSpeed = 4;
    public float targetAlpha = 0;
    private Image image;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        float newAlpha = Mathf.Lerp(image.color.a, targetAlpha, fadeSpeed * Time.deltaTime);
        image.color = new Color(image.color.r, image.color.g, image.color.b, newAlpha);
    }
}
