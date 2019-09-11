using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Letter : MonoBehaviour
{

    public delegate void VoidVector2Param(Vector2 point);
    public static VoidVector2Param onMapSelect;
    public static bool morseMode = false;

    private GraphicRaycaster raycaster;
    private SpriteRenderer sr;
    private Image img;
    private bool isUI;
    private char charValue;
    private MenuNode node;

    // Start is called before the first frame update
    void Awake()
    {
        if(GetComponent<SpriteRenderer>() != null)
        {
            isUI = false;
            sr = GetComponent<SpriteRenderer>();
        }
        else if (GetComponent<Image>() != null)
        {
            raycaster = gameObject.AddComponent<GraphicRaycaster>();
            isUI = true;
            img = GetComponent<Image>();
        }
    }

    private void Start()
    {
        if(morseMode)
            MorseReader.onLetter += _OnActivate;
    }

    private void OnDestroy()
    {
        if(morseMode)
            MorseReader.onLetter -= _OnActivate;
    }

    // Update is called once per frame
    void Update()
    {
        if (!morseMode && isUI && Input.GetKeyDown(KeyCode.Mouse0))
        {
            //Set up the new Pointer Event
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            pointerData.position = Input.mousePosition;
            raycaster.Raycast(pointerData, results);

            //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
            foreach (RaycastResult result in results)
                if(result.gameObject == gameObject)
                    _OnActivate(charValue);
        }
    }

    public void SetNode(MenuNode _node)
    {
        node = _node;
    }

    public void SetLetter(char c)
    {
        c = c.ToString().ToLower()[0];
        charValue = c;
        if(isUI)
            img.sprite = MenuManager.instance.GetSprites()[c - 'a'];
        else
            sr.sprite = MenuManager.instance.GetSprites()[c - 'a'];
    }

    public void OnMouseDown()
    {
        if(!morseMode && !isUI)
            _OnActivate(charValue);
    }

    public void _OnActivate(char c)
    {
        c = c.ToString().ToLower()[0];
        if (c == charValue)
        {
            if (!isUI)
            {
                switch (MenuManager.context)
                {
                    case MenuManager.Context.MapMove:
                        int distance = Grapher.ManhattanDistance(PlayerMover.instance.transform.position, transform.position);
                        bool sprinting = MenuNode.GetCurrent() == "Sprint";

                        PlayerMover.instance.GetComponent<FieldUnit>().ConsumeAP(distance * (sprinting ? 1 : 2));

                        MenuManager.instance.ClearOptions();
                        PlayerMover.instance.GetComponent<Navigator>().SetDestination(transform.position, MenuNode.GetCurrent() == "Sprint");
                        break;
                    case MenuManager.Context.UnitSelect:
                        MenuManager.instance.ClearOptions();
                        Debug.Log("Trying to shoot enemy");
                        break;
                }
                
            }
            if (isUI)
            {
                node.Command();
            }
        }
    }
}
