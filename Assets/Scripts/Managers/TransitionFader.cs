using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionFader : MonoBehaviour
{

    public GameObject rootObject;
    private Image image;
    public static TransitionFader instance;
    private bool fadeToBlack = false;
    private string targetScene = "";

    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(rootObject);
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
                LoadScene(targetScene);
                fadeToBlack = false;
            }
        }
        else if(!fadeToBlack && image.color.a > 0)
        {
            image.color = new Color(0, 0, 0, image.color.a - Time.deltaTime);
        }
    }

    private void LoadScene(string scene)
    {
        if(scene != "TextCutscene" && scene[0] == 'S')
            DialogueParser.sceneToLoad = scene;

        if (Application.CanStreamedLevelBeLoaded(scene))
            SceneManager.LoadScene(scene);
        else if (scene[0] == 'S')
            SceneManager.LoadScene("TextCutscene");
    }

    public void FinishLevel()
    {
        // Update save data
        if(SceneManager.GetActiveScene().name[0] == 'G')
            SaveService.UpdateLevelRecord(SceneManager.GetActiveScene().name,
                    PlayerMover.instance.GetComponent<Inventory>().wallet,
                    Timekeeper.instance.GetSecondsPlayed(),
                    PlayerMover.instance.GetComponent<UnitStatus>().HealthLost());
        else
        {
            if(SceneManager.GetActiveScene().name == "TextCutscene")
                SaveService.UpdateLevelRecord(DialogueParser.sceneToLoad, 0, 0, 0);
            else
                SaveService.UpdateLevelRecord(SceneManager.GetActiveScene().name, 0, 0, 0);
        }

        // Select which scene to load
        if (GameManager.cinemaMode)
        {
            int sceneIndex = Globals.CINEMA_LIST.IndexOf(SceneManager.GetActiveScene().name);
            if (sceneIndex > -1)
                Transition(Globals.CINEMA_LIST[sceneIndex + 1]);
        } 
        else if (SaveService.loadedSave.options.autoplay)
        {
            string activeScene = SceneManager.GetActiveScene().name;
            string scene = activeScene == "TextCutscene" ? DialogueParser.sceneToLoad : activeScene;
            int sceneIndex = Globals.AUTOPLAY_LIST.IndexOf(scene);
            if (sceneIndex > -1)
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
