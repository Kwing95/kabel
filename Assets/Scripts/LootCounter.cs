using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LootCounter : MonoBehaviour
{
    private bool active = true;

    public void Collect()
    {
        if (active)
        {
            MazeMaker.instance.lootLeft -= 1;
            if (MazeMaker.instance.lootLeft <= 0)
            {
                TransitionFader.instance.Transition("Level_Select");
            }
                
            active = false;
        }
    }

}
