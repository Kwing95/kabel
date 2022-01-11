using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HelpPanel : MonoBehaviour, IPointerClickHandler
{

    public static HelpPanel instance;
    public Text message;

    public HelpPanel()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            ButtonHelper.displaying = false;
            gameObject.SetActive(false);
        }
    }
}
