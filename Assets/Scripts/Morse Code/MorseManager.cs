using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This class contains constant variables related to Morse code, including
 * the length of incoming and outcoming longs, shorts, and pauses, as well
 * as a conversion between unicode and Morse characters. */

public class MorseManager : MonoBehaviour
{

    // time (ms) = 1200 / wpm
    public static float displayShortLength = 0.15f;
    public static float farnsworth = 0.1f;

    public static float inputLongLength = 0.18f;
    public static float inputSpaceLength = 0.35f;

    public static readonly List<string> letters = new List<string>(new string[] { ".-",
        "-...", "-.-.", "-..", ".", "..-.", "--.", "....", "..", ".---", "-.-",
        ".-..", "--", "-.", "---", ".--.", "--.-", ".-.", "...", "-", "..-",
        "...-", ".--", "-..-", "-.--", "--.." });
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static char MorseToChar(string morse)
    {
        return (char)('A' + letters.IndexOf(morse));
    }

    public static string CharToMorse(char letter)
    {
        //Debug.Log(letter);
        return letters[letter - 'A'];
    }

}
