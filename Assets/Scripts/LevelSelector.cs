using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelData
{
    public string title;
    public bool playable;
    public string scene;
    public bool hasHardmode;

    public LevelData(string _title, bool _playable, string _scene, bool _hasHardmode)
    {
        title = _title;
        playable = _playable;
        scene = _scene;
        hasHardmode = _hasHardmode;
    }
}

public class LevelSelector : MonoBehaviour
{

    public Image autoplayImage;
    public Image hardmodeImage;
    public GameObject levelGrid;
    public GameObject levelDetails;
    public Text chapterText;
    public Text longDescription;

    public static LevelSelector instance;

    private static bool hardmode = false;
    private static int currentChapter = 1;
    private static string selectedLevel = "";
    private static List<List<LevelData>> levelList;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        if (levelList == null)
            BuildLevels();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void BuildLevels()
    {
        levelList = new List<List<LevelData>>();
        for(int i = 0; i < Globals.NUM_CHAPTERS; ++i)
            levelList.Add(new List<LevelData>());

        levelList[0].Add(new LevelData("Intro", false, "S1-1", false));
        levelList[0].Add(new LevelData("Infiltration A", true, "G1-1", false));
        levelList[0].Add(new LevelData("Infiltration B", true, "G1-2", false));
        levelList[0].Add(new LevelData("Infiltration C", true, "G1-3", false));
        levelList[0].Add(new LevelData("Innocents", false, "S1-2", false));
        levelList[0].Add(new LevelData("Escape A", true, "G1-4", false));
        levelList[0].Add(new LevelData("Escape B", true, "G1-5", false));
        levelList[0].Add(new LevelData("Escape C", true, "G1-6", false));
        levelList[0].Add(new LevelData("Infirmary", false, "S1-3", false));

    }

    public void PlayLevel()
    {
        TransitionFader.instance.Transition(selectedLevel);
    }

    public void ToggleAutoplay()
    {
        SaveService.loadedSave.options.autoplay = !SaveService.loadedSave.options.autoplay;
        autoplayImage.color = new Color(1, 1, 1, SaveService.loadedSave.options.autoplay ? 1 : 0.5f);
        Debug.Log("Autoplay is " + (SaveService.loadedSave.options.autoplay ? "on" : "off"));
        Toast.ToastWrapper("Autoplay is " + (SaveService.loadedSave.options.autoplay ? "on" : "off"));
    }

    public void ToggleHardmode()
    {
        hardmode = !hardmode;
        hardmodeImage.color = new Color(1, 1, 1, hardmode ? 1 : 0.5f);
        Toast.ToastWrapper(hardmode ? "Showing hardmode levels" : "Showing normal levels");
    }

    public void ChapterSelect(bool next)
    {
        currentChapter = Mathf.Clamp(currentChapter + (next ? 1 : -1), 1, Globals.NUM_CHAPTERS);
        chapterText.text = "CHAPTER " + currentChapter.ToString();
        // Toast.ToastWrapper("Chapter " + currentChapter);
    }

    public static bool GetHardmode()
    {
        return hardmode;
    }

    public static int GetCurrentChapter()
    {
        return currentChapter;
    }

    public void ShowLevelGrid()
    {
        levelGrid.SetActive(true);
        levelDetails.SetActive(false);
    }

    public void SetSelectedLevel(string level)
    {
        levelGrid.SetActive(false);
        levelDetails.SetActive(true);

        selectedLevel = level; // currentChapter + "-" + level + (hardmode ? "H" : "");
        // Debug.Log(selectedLevel);
        longDescription.text = GetLevelDescription(selectedLevel);
    }

    public static string GetLevelDescription(string levelName)
    {
        string output = "";
        
        // Get level index and scene name
        // Debug.Log(Globals.AUTOPLAY_LIST.IndexOf(levelName));
        LevelStats data = SaveService.loadedSave.levels[Globals.AUTOPLAY_LIST.IndexOf(levelName)];
        output += Globals.LEVEL_DATA[levelName].scene + " " + Globals.LEVEL_DATA[levelName].title + "\n";

        if (Globals.LEVEL_DATA[levelName].playable)
        {
            if (data.bestTime == -1)
                output += "\nUNCLEARED\n\n\n\n";
            else
            {
                output += "\nCLEARED\n\n";
                output += "Clear Time: " + data.bestTime.ToString() + "\n";
                output += "Damage Taken: " + data.healthLost.ToString() + "\n";
                output += "Money Collected: " + data.moneyCollected.ToString() + "\n\n";
            }
        }
        else
            output += "\nCUTSCENE\n\n\n";

        return output;
    }

}
