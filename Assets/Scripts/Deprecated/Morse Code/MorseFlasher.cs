using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This component modifies the opacity of an object in rhythm
 * with any Morse code message. Call AddToBuffer(string message)
 * to make the object flash. */

public class MorseFlasher : MonoBehaviour
{

    private float shortLength;
    private float farnsworth;
    private float longLength;
    private float symbolSpaceLength;
    private float letterSpaceLength;
    private float wordSpaceLength;
    
    // private bool visible = false;

    private string messageBuffer = "";
    private string charBuffer = "";

    private float waitTime = 0;
    private bool needsGap = false;
    private float counter = 0;
    private new CanvasRenderer renderer;

    // OnLetter is beginning a new letter
    // OnTone is executing longs and shorts inside of a letter
    private enum State { OnLetter, OnTone };
    // private State state = State.OnLetter;

    // Start is called before the first frame update
    void Start()
    {
        shortLength = MorseManager.displayShortLength;
        farnsworth = MorseManager.farnsworth;

        longLength = shortLength * 3;
        symbolSpaceLength = shortLength + farnsworth;
        letterSpaceLength = (shortLength + farnsworth) * 3;
        wordSpaceLength = (shortLength + farnsworth) * 7;

        renderer = GetComponent<CanvasRenderer>();
        // AddToBuffer("ABC");
    }

    // Update is called once per frame
    void Update()
    {

        counter += Time.deltaTime;

        // If waiting for tone to expire, or buffer is empty
        if (counter < waitTime)
            return;

        // If buffer is empty, flasher shuts off
        if (messageBuffer == "")
        {
            Hide();
            return;
        }

        counter = 0;

        // If a symbol has finished displaying
        if (needsGap)
        {
            //Debug.Log(messageBuffer);
            Hide();

            //Debug.Log("'" + charBuffer + "' '" + messageBuffer[0] + "'");

            // Next character is a space and current character just finished
            if (charBuffer == "" && messageBuffer[0] == ' ')
            {
                //Debug.Log("word space");
                //Debug.Log(messageBuffer + " " + charBuffer);
                waitTime = wordSpaceLength;
                messageBuffer = Chop(messageBuffer);
                charBuffer = messageBuffer.Length > 0 ? MorseManager.CharToMorse(messageBuffer[0]) : ""; // error if text ends in space
                messageBuffer = Chop(messageBuffer);
                //Debug.Log(messageBuffer + " " + charBuffer);
            }
            // Done outputting character, load next one
            else if (charBuffer == "")
            {
                //Debug.Log("char space");
                //Debug.Log(messageBuffer);
                waitTime = letterSpaceLength;
                charBuffer = MorseManager.CharToMorse(messageBuffer[0]);
                //Debug.Log(charBuffer);
                messageBuffer = Chop(messageBuffer);
            }
            else
            {
                //Debug.Log("symbol space");
                waitTime = symbolSpaceLength;
            }

            needsGap = false;
            return;
        }

        // If a gap has finished displaying, display next char
        switch (charBuffer[0])
        {
            case '-':
                waitTime = longLength;
                //Debug.Log("long");
                break;
            case '.':
                //Debug.Log("short");
                waitTime = shortLength;
                break;
        }
        Show();
        charBuffer = Chop(charBuffer);
        needsGap = true;

    }

    // Chop off the first character of a string
    public string Chop(string s)
    {
         return s.Length > 1 ? s.Substring(1) : "";
    }

    public void AddToBuffer(string message)
    {
        if(messageBuffer == "")
            needsGap = true;

        messageBuffer += message + " ";
    }

    private void Show()
    {
        renderer.SetAlpha(255);
    }

    private void Hide()
    {
        renderer.SetAlpha(0);
    }
}
