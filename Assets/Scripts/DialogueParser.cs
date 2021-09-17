using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueParser : MonoBehaviour
{
    public static DialogueParser instance;

    public string sceneName;
    public TextMeshProUGUI dialogueBox;
    public bool isCutscene = true;

    private List<string> dialogueList;
    private int currentLine = 0;

    public DialogueParser()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Awake()
    {
        dialogueList = new List<string>();
        ParseScene(sceneName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AdvanceText(int increment)
    {
        dialogueBox.pageToDisplay += increment;
        if(dialogueBox.pageToDisplay == 0)
            IncrementLine(-1);
        else if(dialogueBox.pageToDisplay > dialogueBox.textInfo.pageCount)
            IncrementLine(1);
    }

    private void IncrementLine(int increment)
    {
        if(currentLine == dialogueList.Count - 1 && increment == 1)
            CloseDialogue();
        else
        {
            currentLine = Mathf.Clamp(currentLine + increment, 0, dialogueList.Count - 1);
            dialogueBox.text = dialogueList[currentLine];
            dialogueBox.pageToDisplay = 1;
        }
    }

    private void CloseDialogue()
    {
        if (isCutscene)
        {
            dialogueBox.pageToDisplay = dialogueBox.textInfo.pageCount;
            TransitionFader.instance.FinishLevel();
        }
        else
        {
            Sidebar.instance.ToggleInGameDialogue(false);
            dialogueBox.pageToDisplay = 1;
        }
    }

    public void SetDialogue(string text)
    {
        dialogueList = new List<string>();
        dialogueList.Add(text);

        currentLine = 0;
        dialogueBox.text = dialogueList[0];
    }

    public void ParseScene(string _sceneName)
    {
        int startIndex = Globals.GAME_SCRIPT.text.IndexOf("<" + _sceneName + ">") + 3 + _sceneName.Length;
        int endIndex = Globals.GAME_SCRIPT.text.IndexOf("</" + _sceneName + ">");
        string dump = Globals.GAME_SCRIPT.text.Substring(startIndex, endIndex - startIndex);
        // Use \r\n for Windows and use \n for Linux
        dialogueList = dump.Split(new [] { "\r\n\r\n" }, StringSplitOptions.None).ToList();
        dialogueList[0] = dialogueList[0].TrimStart('\n');

        currentLine = 0;
        dialogueBox.text = dialogueList[0];
    }

}
