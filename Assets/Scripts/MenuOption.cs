using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuOption : MonoBehaviour
{

    public Letter menuLetter;
    public Text menuLabel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetOption(MenuNode node, char letter)
    {
        menuLabel.text = node.GetName();
        menuLetter.SetLetter(letter);
        menuLetter.SetNode(node);
    }

}
