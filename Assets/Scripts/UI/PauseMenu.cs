using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    [SerializeField]
    public enum Context { Restart, Quit, Skip };
    public Context state = Context.Restart;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Confirm()
    {
        switch (state)
        {
            case Context.Restart:
                RestartLevel();
                break;
            case Context.Quit:
                QuitToMenu();
                break;
            case Context.Skip:
                TransitionFader.instance.FinishLevel();
                break;
        }
    }

    public void TogglePauseMenu()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void RestartLevel()
    {
        TransitionFader.instance.ReloadScene();
    }

    public void QuitToMenu()
    {
        TransitionFader.instance.Transition("Level_Select"); // Main menu scene
    }

    public void SetContext(string newContext)
    {
        state = (Context)Enum.Parse(typeof(Context), newContext);
    }

    // VD = Voice down, MU = Music up
    public void AdjustVolume(string args)
    {
        if (args.Length != 2)
            return;

        float increment = args[1] == 'U' ? 0.1f : -0.1f;

        // Voice/Music/Sounds
        switch (args[0]){
            case 'V':
                SaveService.loadedSave.options.voiceVolume = Mathf.Clamp(SaveService.loadedSave.options.voiceVolume + increment, 0, 1);
                break;
            case 'M':
                SaveService.loadedSave.options.musicVolume = Mathf.Clamp(SaveService.loadedSave.options.musicVolume + increment, 0, 1);
                break;
            case 'S':
                SaveService.loadedSave.options.soundVolume = Mathf.Clamp(SaveService.loadedSave.options.soundVolume + increment, 0, 1);
                break;
        }

    }

}
