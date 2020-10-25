using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionFader : MonoBehaviour
{

    private Image image;
    public static TransitionFader instance;
    private bool fadeToBlack = false;
    private int targetScene = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
        {
            instance = this;
            DontDestroyOnLoad(transform.parent.gameObject);
            DontDestroyOnLoad(this);
        }

        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeToBlack && image.color.a < 1)
        {
            image.color = new Color(0, 0, 0, image.color.a + Time.deltaTime);
            if(image.color.a >= 1)
            {
                SceneManager.LoadScene(targetScene);
                fadeToBlack = false;
            }
        }
        else if(!fadeToBlack && image.color.a > 0)
        {
            image.color = new Color(0, 0, 0, image.color.a - Time.deltaTime);
        }
    }

    public void Transition(int scene)
    {
        if (!fadeToBlack)
        {
            Time.timeScale = 1;
            fadeToBlack = true;
            targetScene = scene;
        }
    }

}
