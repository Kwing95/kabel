using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueParser : MonoBehaviour
{
    public string sceneName;
    public TextMeshProUGUI dialogueBox;

    private List<string> dialogueList;
    private int currentLine = 0;

    // Start is called before the first frame update
    void Start()
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
        {
            dialogueBox.pageToDisplay = dialogueBox.textInfo.pageCount;
            TransitionFader.instance.FinishLevel();
            return;
        }

        currentLine = Mathf.Clamp(currentLine + increment, 0, dialogueList.Count - 1);
        dialogueBox.text = dialogueList[currentLine];
        dialogueBox.pageToDisplay = 1;
    }

    private bool CheckTextWidth(string text)
    {
        return false;
        /*float preferredWidth = LayoutUtility.GetPreferredWidth(text.rectTransform);
        float parentWidth = parentRect.rect.width;
        return (preferredWidth > (parentWidth - longestCharWidth));*/
    }

    private void ParseScene(string _sceneName)
    {
        int startIndex = Globals.GAME_SCRIPT.text.IndexOf("<" + _sceneName + ">") + 2 + _sceneName.Length;
        int endIndex = Globals.GAME_SCRIPT.text.IndexOf("</" + _sceneName + ">");
        string dump = Globals.GAME_SCRIPT.text.Substring(startIndex, endIndex - startIndex);
        dialogueList = dump.Split(new[] { "\n\n" }, StringSplitOptions.None).ToList();
        dialogueList[0] = dialogueList[0].TrimStart('\n');

        currentLine = 0;
        dialogueBox.text = dialogueList[0];
    }

}
