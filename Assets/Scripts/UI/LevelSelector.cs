using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    public enum Context { Title, Levels, Extras, Options, Arcade };
    private Context context = Context.Title;
    public InputField customSeed;

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
        DialogueParser.sceneToLoad = selectedLevel;

        if (Application.CanStreamedLevelBeLoaded(selectedLevel))
            TransitionFader.instance.Transition(selectedLevel);
        else if (selectedLevel[0] == 'S')
            TransitionFader.instance.Transition("TextCutscene");
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
                // Show only if level is unlocked
                foreach(LevelRecord record in SaveService.loadedSave.levels)
                    if(record.levelId == level.levelId && record.unlocked)
                    {

                    }
                // Always show
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

    private static LevelData GetLevel(string levelName)
    {
        foreach (LevelData level in Globals.LEVEL_LIST)
            if (level.levelId == levelName)
                return level;

        return new LevelData("ERROR", "ERROR");
    }

    public static string GetLevelDescription(string levelName)
    {
        string output = "";

        // Fetch data pertaining to the level itself
        LevelData levelInfo = GetLevel(levelName);
        // Fetch player save data relating to the level
        LevelRecord savedLevelData = SaveService.loadedSave.levels[Globals.AUTOPLAY_LIST.IndexOf(levelName)];

        output += levelInfo.levelId + " " + levelInfo.levelName + "\n";

        if (levelInfo.isPlayable)
        {
            if (savedLevelData.bestTime == -1)
                output += "\nUNCLEARED\n\n\n\n";
            else
            {
                output += "\nCLEARED\n\n";
                output += "Clear Time: " + savedLevelData.bestTime + "\n";
                output += "Damage Taken: " + savedLevelData.healthLost + "\n";
                output += "Money Collected: " + savedLevelData.loot + "\n\n";
            }
        }
        else
            output += "\nCUTSCENE\n\n\n";

        return output;
    }

    public void RandomizeSeed()
    {
        MazeMaker.seed = UnityEngine.Random.Range(1, 10000000);
        customSeed.text = MazeMaker.seed.ToString();
    }

    public void StartArcade()
    {
        try
        {
            MazeMaker.seed = int.Parse(customSeed.text);
            TransitionFader.instance.Transition("RandomMap");
        }
        catch (FormatException)
        {
            Toast.ToastWrapper("Seed must be a number");
            customSeed.text = "";
        }
    }

}
