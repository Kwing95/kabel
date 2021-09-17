using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{

    public Text buttonLabel;
    public string levelId;
    public Image movieIcon;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(string label, string _levelId)
    {
        if (_levelId[0] == 'S')
            movieIcon.enabled = true;
        else
            buttonLabel.text = label;
        
        levelId = _levelId;
    }

    public void SelectLevel()
    {
        LevelSelector.instance.SetSelectedLevel(levelId);
    }

}
