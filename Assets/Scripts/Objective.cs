using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{

    public SpriteRenderer outline;
    public bool levelFinish = true;
    public string message = "";
    public bool messageFromScene = false;
    private bool active = false;

    // Start is called before the first frame update
    void Start()
    {
        outline.color = levelFinish ? new Color(1, 0.8f, 0) : new Color(0, 0.8f, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (!active && Vector2.Distance(transform.position, PlayerMover.instance.transform.position) < 0.5f)
        {
            active = true;
            if (levelFinish)
                TransitionFader.instance.FinishLevel();
            else
            {
                Sidebar.instance.ToggleInGameDialogue(true);

                if (messageFromScene)
                    DialogueParser.instance.ParseScene(message);
                else
                    DialogueParser.instance.SetDialogue(message);
            }  
        }
        // Reset for repeatable scenes
        else if(active && Vector2.Distance(transform.position, PlayerMover.instance.transform.position) > 0.5f)
            active = false;
    }
}
