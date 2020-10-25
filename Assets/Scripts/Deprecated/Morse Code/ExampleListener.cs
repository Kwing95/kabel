using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExampleListener : MonoBehaviour
{

    public delegate void VoidBoolParam(bool b);
    public VoidBoolParam keyDownListener;

    private string output;

    // Start is called before the first frame update
    void Start()
    {
        output = "DEFAULT";
        keyDownListener += _OnKeyDown;
        //keyDownListener += _SecondHandler;
    }

    // Update is called once per frame
    void Update()
    {
        keyDownListener(Input.GetKey("down"));
        //Debug.Log(output);
    }

    void _OnKeyDown(bool b)
    {
        if (b)
        {
            output = "Key is pressed. ";
        }
        else
        {
            output = "Key is not pressed. ";
        }
    }

    void _SecondHandler(bool b)
    {
        if (b)
        {
            output = output + ":)";
        }
        else
        {
            output = output + ":(";
        }
    }
}
