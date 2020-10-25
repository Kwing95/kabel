using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandHandler : MonoBehaviour
{

    public delegate void action(string s);
    private List<action> commands = new List<action>();

    // Start is called before the first frame update
    void Start()
    {
        action c = new action(Logger);
        action b = new action(ThrowGrenade);
        List<action> options = new List<action>();
        options.Add(c);
        options.Add(b);
        MakeMenu(options);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReceiveCommand()
    {

    }

    public void MakeMenu(List<action> options)
    {
        // Remove old menu
        ResetCommands();

        // Reset which letters are available
        List<int> openLetters = new List<int>(new int[] {0, 1, 2, 3, 4, 5, 6, 7,
            8, 9, 10, 11, 12, 13, 14, 15, 16,
            17, 18, 19, 20, 21, 22, 23, 24, 25});

        // Assign each option to a random letter
        for (int i = 0; i < options.Count; ++i)
        {
            int randomLetter = Random.Range(0, openLetters.Count);
            commands[randomLetter] = options[i];
            openLetters.RemoveAt(randomLetter);
        }

    }

    private void ResetCommands()
    {
        commands.Clear();
        while (commands.Count < 26)
            commands.Add(Invalid);
    }

    public void Logger(string s)
    {
        Debug.Log(s);
    }

    private void ThrowGrenade(string s)
    {
        Displayer.instance.DisplayMessage("Threw a grenade!", 2);
    }

    private void Invalid(string s)
    {
        Displayer.instance.DisplayMessage("Invalid command!", 3);
    }

}
