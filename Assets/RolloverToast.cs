using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RolloverToast : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{

    public string label = "";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSelect(BaseEventData eventData=null)
    {
        //if (eventData.selectedObject && eventData.selectedObject == gameObject)
        //{
            Toast.ToastWrapper(label);
            Debug.Log(gameObject.name + " was selected");
        //}
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnSelect();
    }

}
