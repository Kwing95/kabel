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
    private string targetScene = "";

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

    public void FinishLevel()
    {
        if (GameManager.cinemaMode)
        {
            int sceneIndex = Globals.CINEMA_LIST.IndexOf(SceneManager.GetActiveScene().name);
            if (sceneIndex > -1)
                Transition(Globals.CINEMA_LIST[sceneIndex + 1]);
        } 
        else if (SaveService.loadedSave.options.autoplay)
        {
            int sceneIndex = Globals.AUTOPLAY_LIST.IndexOf(SceneManager.GetActiveScene().name);
            if(sceneIndex > -1)
                Transition(Globals.AUTOPLAY_LIST[sceneIndex + 1]);
        }
        else
            Transition("Level_Select");
    }

    public void Transition(string scene)
    {
        if (!fadeToBlack)
        {
            Time.timeScale = 1;
            fadeToBlack = true;
            targetScene = scene;
        }
    }

    public void ReloadScene()
    {
        Transition(SceneManager.GetActiveScene().name);
    }

}
