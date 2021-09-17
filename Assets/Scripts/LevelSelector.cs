using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelData
{
    public readonly string levelName;
    public readonly string levelId;

    public readonly bool isPlayable;
    public readonly int chapter;
    public readonly int orderInChapter;
    public readonly bool isHardmode;

    public LevelData(string _levelName, string _levelId)
    {
        levelName = _levelName;
        levelId = _levelId;
        isPlayable = _levelId[0] == 'G';
        int dashIndex = _levelId.IndexOf('-');
        chapter = int.Parse(_levelId.Substring(1, dashIndex - 1));
        orderInChapter = int.Parse(_levelId.Substring(dashIndex + 1, _levelId.Length - (dashIndex + 1)));
        isHardmode = _levelId[_levelId.Length - 1] == 'H';
    }
}

public class LevelSelector : MonoBehaviour
{

    public enum Context { Title, Levels, Extras, Options };
    private Context context = Context.Title;

    public List<GameObject> menuContent;

    public Image autoplayImage;
    public Image hardmodeImage;

    public GameObject leftMenu;
    public GameObject rightMenu;

    public GameObject levelGrid;
    public GameObject buttonContainer;
    public GameObject levelDetails;

    public Text chapterText;
    public Text longDescription;

    public static LevelSelector instance;

    private static bool hardmode = false;
    private static int currentChapter = 1;
    private static string selectedLevel = "";

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        hardmodeImage.color = new Color(1, 1, 1, hardmode ? 1 : 0.5f);
        autoplayImage.color = new Color(1, 1, 1, 1);
        ShowLevels(1, false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetContext(string newContext)
    {
        Context contextEnum = (Context)Enum.Parse(typeof(Context), newContext);
        
        for (int i = 0; i < menuContent.Count; ++i)
            menuContent[i].SetActive((int)contextEnum == i);
    }

    public void PlayLevel()
    {
        TransitionFader.instance.Transition(selectedLevel);
    }

    public void ToggleAutoplay()
    {
        SaveService.loadedSave.options.autoplay = !SaveService.loadedSave.options.autoplay;
        autoplayImage.color = new Color(1, 1, 1, SaveService.loadedSave.options.autoplay ? 1 : 0.5f);
        Toast.ToastWrapper("Autoplay is " + (SaveService.loadedSave.options.autoplay ? "on" : "off"));
    }

    public void ToggleHardmode()
    {
        hardmode = !hardmode;
        Toast.ToastWrapper("Hardmode is " + (hardmode ? "on" : "off"));
        hardmodeImage.color = new Color(1, 1, 1, hardmode ? 1 : 0.5f);
        ShowLevels(currentChapter, hardmode);
    }

    public void ChapterSelect(bool next)
    {
        currentChapter = Mathf.Clamp(currentChapter + (next ? 1 : -1), 1, Globals.NUM_CHAPTERS);
        ShowLevels(currentChapter, hardmode);
        chapterText.text = "CHAPTER " + currentChapter.ToString();
    }

    public static bool GetHardmode()
    {
        return hardmode;
    }

    public static int GetCurrentChapter()
    {
        return currentChapter;
    }

    private void ToggleGridDisplay(bool showGrid)
    {
        rightMenu.SetActive(showGrid);
        leftMenu.SetActive(showGrid);
        levelGrid.SetActive(showGrid);
        levelDetails.SetActive(!showGrid);
    }

    public void ShowLevelGrid()
    {
        ToggleGridDisplay(true);
    }

    public void SetSelectedLevel(string level)
    {
        ToggleGridDisplay(false);

        selectedLevel = level;
        longDescription.text = GetLevelDescription(selectedLevel);
    }

    public void ShowLevels(int chapter, bool isHardmode)
    {
        ClearLevelButtons();

        foreach(LevelData level in Globals.LEVEL_LIST)
            if(level.chapter == chapter && level.isHardmode == isHardmode)
            {
                foreach(LevelRecord record in SaveService.loadedSave.levels)
                {
                    if(record.levelId == level.levelId && record.unlocked)
                    {

                    }
                }
                GameObject newButton = Instantiate(Globals.LEVEL_BUTTON, buttonContainer.transform);
                newButton.GetComponent<LevelButton>().Initialize(level.orderInChapter.ToString(), level.levelId);
            }
    }

    public void ClearLevelButtons()
    {
        foreach (Transform child in buttonContainer.transform)
            Destroy(child.gameObject);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public static string GetLevelDescription(string levelName)
    {
        string output = "";
        
        // Get level index and scene name
        // Debug.Log(Globals.AUTOPLAY_LIST.IndexOf(levelName));
        LevelRecord data = SaveService.loadedSave.levels[Globals.AUTOPLAY_LIST.IndexOf(levelName)];
        output += Globals.LEVEL_DATA[levelName].levelId + " " + Globals.LEVEL_DATA[levelName].levelName + "\n";

        if (Globals.LEVEL_DATA[levelName].isPlayable)
        {
            if (data.bestTime == -1)
                output += "\nUNCLEARED\n\n\n\n";
            else
            {
                output += "\nCLEARED\n\n";
                output += "Clear Time: " + data.bestTime + "\n";
                output += "Damage Taken: " + data.healthLost + "\n";
                output += "Money Collected: " + data.loot + "\n\n";
            }
        }
        else
            output += "\nCUTSCENE\n\n\n";

        return output;
    }

}
