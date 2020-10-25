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
    private Sprite current;
    private Sprite nullSprite;
    private bool isUI;
    private char charValue;
    private MenuNode node;

    // Start is called before the first frame update
    void Awake()
    {
        if (GetComponent<SpriteRenderer>() != null)
        {
            isUI = false;
            sr = GetComponent<SpriteRenderer>();

            Color c = sr.color;
            c.a = 0.5f;
            sr.color = c;
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
        MorseReader.onLetter += _OnActivate;
    }

    private void OnDestroy()
    {
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
        current = MenuManager.instance.GetSprites()[c - 'a'];

        if(isUI)
            img.sprite = morseMode ? current : MenuManager.instance.GetSprites()[26];
        else
            sr.sprite = morseMode ? current : MenuManager.instance.GetSprites()[26];
    }

    public void OnMouseDown()
    {
        if(!morseMode && !isUI)
            _OnActivate(charValue);
    }

    public void RefreshSprite()
    {
        SetLetter(charValue);
    }

    public void _OnActivate(char c)
    {
        c = c.ToString().ToLower()[0];
        if (c == charValue)
        {
            if (!isUI)
            {
                CommandManager.MapCommand(MenuManager.context, transform.position);
            }
            if (isUI)
            {
                node.Command();
            }
        }
    }
}
