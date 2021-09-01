using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [ExecuteInEditMode]
public class GameManager : MonoBehaviour
{

    // Bane, Biggs, Saito, Wedge
    // Asra, Diego, Thane
    // Diego, Jacques, Thane

    public static bool cinemaMode = false;

    /*
    public static float voiceVolume = 0.5f;
    public static float musicVolume = 0.5f;
    public static float soundVolume = 0.5f;

    public static int actionPoints = 5;
    public static bool running = false;

    public static int[] health = { 3, 3, 3 };
    public static int[] focus = { 3, 3, 3 };

    private static int numGauze = 3;
    private static int numSalts = 3;
    public static int numFrag = 0;
    public static int numSmoke = 0;*/

    // Start is called before the first frame update
    void Start()
    {
        #if UNITY_EDITOR
            Screen.SetResolution(1920, 1080, false);
            QualitySettings.vSyncCount = 0;  // VSync must be disabled
            Application.targetFrameRate = 45;
        #endif

        Screen.orientation = ScreenOrientation.LandscapeLeft;
        if (SaveService.loadedSave == null)
        {
            SaveService.loadedSave = SaveService.LoadData();
            Debug.Log(SaveService.loadedSave);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    /*public void SaveWrapper()
    {
        SaveService.SaveData(loadedSave);
    }

    public void LoadWrapper()
    {
        loadedSave = SaveService.LoadData();
    }

    public void SaveChanger()
    {
        loadedSave.options.musicVolume += 0.1f;
        Debug.Log("musicVolume: " + loadedSave.options.musicVolume.ToString());
        Toast.ToastWrapper("musicVolume: " + loadedSave.options.musicVolume.ToString());
    }*/

}