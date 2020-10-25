using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This class converts the player's Morse code input into
 * letters that can then be used to execute in-game commands. */

public class MorseReader : MonoBehaviour
{

    public delegate void VoidCharParam(char letter);
    public static VoidCharParam onLetter;

    private float minLongLength;
    private float minSpaceLength;

    private CommandHandler handler;
    private float timeDown = 0;
    private float timeUp = 0;
    private bool pressed = false;
    private string inputLetter = "";
    private bool confirmMode = false;

    public Displayer displayer;

    // Start is called before the first frame update
    void Start()
    {
        minLongLength = MorseManager.inputLongLength;
        minSpaceLength = MorseManager.inputSpaceLength;
        
        handler = GetComponent<CommandHandler>();
        //GameManager.morseListener += 0;
    }

    // Update is called once per frame
    void Update()
    {

        if (pressed)
            timeDown += Time.deltaTime;
        else
        {
            timeUp += Time.deltaTime;
            if (timeUp >= minSpaceLength && inputLetter.Length > 0)
                SendLetter();
        }

        if (Input.GetMouseButtonDown(0))
            Press();
        if (Input.GetMouseButtonUp(0))
            Release();

        if (Input.GetKeyDown("space"))
            Press();
        if (Input.GetKeyUp("space"))
            Release();
    }

    private void SendLetter()
    {
        
        //CommandHandler.Receive

        char letterTyped = MorseManager.MorseToChar(inputLetter);
        if (letterTyped == 'A' - 1)
            Displayer.instance.DisplayMessage("Invalid", 2);
        else
        {
            if(Letter.morseMode)
                onLetter(letterTyped);
        }
            

        inputLetter = "";
    }

    private void Press()
    {
        timeUp = timeDown = 0;
        pressed = true;
    }

    private void Release()
    {
        if (timeDown >= minLongLength)
            inputLetter += "-";
        else
            inputLetter += ".";
        displayer.SetValue(inputLetter);
        // Max length is 5 normally, changes to 1 for confirm mode
        if (inputLetter.Length >= (confirmMode ? 1 : 5))
            SendLetter();
 
        timeUp = timeDown = 0;
        pressed = false;
    }

    public void SetConfirmMode(bool value)
    {
        confirmMode = value;
    }

    public void _OnMorseLetter(char letter)
    {

    }

}
