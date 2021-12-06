using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootCounter : MonoBehaviour
{

    private static int numLoot = 0;
    private bool active = true;

    // Start is called before the first frame update
    void Start()
    {
        numLoot += 1;
    }

    public void Collect()
    {
        if (active)
        {
            numLoot -= 1;
            if (numLoot <= 0)
                TransitionFader.instance.Transition("Level_Select");
            active = false;
        }
    }

    private void OnDestroy()
    {
        numLoot -= 1;
    }

}
