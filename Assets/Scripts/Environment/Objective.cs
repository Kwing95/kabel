using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Objective : MonoBehaviour
{

    public SpriteRenderer outline;
    public bool levelFinish = true;
    public string messageId = "";
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
            {
                if (AutoMover.InAlertStatus())
                {
                    Toast.ToastWrapper("Evade or eliminate enemies to proceed");
                }
                else
                {
                    PlayerMover.instance.GetComponent<Navigator>().Pause();
                    PlayerMover.instance.GetComponent<BoxCollider2D>().enabled = false;

                    TransitionFader.instance.FinishLevel();
                }
            }
            else
            {
                Sidebar.instance.ToggleInGameDialogue(true);
                DialogueParser.instance.ParseScene(messageId);
            }  
        }
        // Reset for repeatable scenes
        else if(active && Vector2.Distance(transform.position, PlayerMover.instance.transform.position) > 0.5f)
            active = false;
    }
}
