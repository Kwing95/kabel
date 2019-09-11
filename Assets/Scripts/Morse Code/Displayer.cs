using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* This component modifies a text field, allowing data to
 * be printed to the screen. */

public class Displayer : MonoBehaviour
{

    private static Text display;
    public static Displayer instance;

    // Start is called before the first frame update
    void Start()
    {

        MorseReader.onLetter += _OnMorseCharacter;

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(this);
        
        display = GetComponent<Text>();
    }

    private void OnDestroy()
    {
        MorseReader.onLetter -= _OnMorseCharacter;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DisplayMessage(string message, float time)
    {
        StartCoroutine(MessageCoroutine(message, time));
    }

    private static IEnumerator MessageCoroutine(string message, float time)
    {
        display.text = message;
        yield return new WaitForSeconds(time);
        if (display.text == message)
            display.text = "";
    }

    public string GetValue()
    {
        return display.text;
    }

    public void SetValue(string value)
    {
        display.text = value;
    }

    public void _OnMorseCharacter(char letter)
    {
        DisplayMessage(letter.ToString(), 2);
    }

}
