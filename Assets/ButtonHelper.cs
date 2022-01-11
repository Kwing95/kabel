using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHelper : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{

    public string message = "Default message.";
    private float timeToHold = 0.5f;
    private float timeDown = 0;
    private bool pressing = false;
    public static bool displaying = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (pressing && !displaying)
        {
            timeDown += Time.unscaledDeltaTime;
            if (timeDown >= timeToHold)
                OpenDisplay();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pressing = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pressing = false;
        timeDown = 0;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right && !displaying)
            OpenDisplay();
    }

    private void OpenDisplay()
    {
        HelpPanel.instance.gameObject.SetActive(true);
        HelpPanel.instance.message.text = message;
        displaying = true;
    }

}
