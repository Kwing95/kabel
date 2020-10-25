using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuNode
{
    public char character;
    public enum NavigationType { Stay, Close, Parent, Child };

    private static MenuNode current;
    private MenuNode parent;
    private List<MenuNode> children;

    private NavigationType navigationType;
    private string itemName = "";

    public MenuNode()
    {
        children = new List<MenuNode>();
    }

    public MenuNode(string _itemName, MenuNode _parent, NavigationType _navigationType)
    {
        children = new List<MenuNode>();

        itemName = _itemName;
        parent = _parent == null ? this : _parent; // Node defaults to being its own parent
        navigationType = _navigationType;
    }

    public void Command()
    {

        // maybe navigationType for map letters - target/move?
        switch (navigationType)
        {
            case NavigationType.Child:
                MenuManager.instance.ShowMenu(children);
                current = this;
                break;
            case NavigationType.Parent:
                current = parent.parent;
                MenuManager.instance.ShowMenu(current.children);
                break;
            case NavigationType.Stay:
                MenuManager.instance.ShowMenu(parent.children);
                break;
            case NavigationType.Close:
                break;
            default:
                break;
        }

        // If a menu item has children, selecting it should open up its submenu
        switch (itemName)
        {
            case "Walk":
                MenuManager.context = MenuManager.Context.MapMove;
                MenuManager.instance.ShowMenu(children);
                current = this;
                // NOTE: Need to get AP from Navigator component; Navigator needs to handle AP
                MenuManager.instance.MoveMenu(PlayerMover.instance.transform.position);
                break;
            case "Sprint":
                MenuManager.context = MenuManager.Context.MapMove;
                MenuManager.instance.ShowMenu(children);
                current = this;
                MenuManager.instance.MoveMenu(PlayerMover.instance.transform.position);
                break;
            case "Firearm":
                MenuManager.context = MenuManager.Context.UnitSelect;
                MenuManager.instance.ShowMenu(children);
                current = this;
                MenuManager.instance.TargetMenu(PlayerMover.instance.transform.position, 10);
                break;
            case "End Turn":
                break;
            case "Morse Mode":
                MenuManager.instance.SetMorse(!Letter.morseMode);
                MenuManager.instance.ShowMenu(parent.children);
                break;
            case "root":
                //Debug.Log("Executed root menu.");
                break;
        }

    }

    public static void SetCurrent(MenuNode node)
    {
        current = node;
    }

    public void Add(MenuNode newNode)
    {
        children.Add(newNode);
    }

    public void AddBack()
    {
        children.Add(new MenuNode("Back", this, NavigationType.Parent));
    }

    public static string GetCurrent()
    {
        return current.itemName;
    }

    public string GetName()
    {
        return itemName;
    }

    public static void RefreshMenu()
    {
        current.Command();
    }

}

// CommandManager

public class CommandManager : MonoBehaviour
{

    // Use "None" if option only navigates menu
    public enum MenuNavigation { Parent, Child, Close, Stay };
    public enum MenuFunction { None, NewGame, Options, Back, Walk, Sprint, Target, EndTurn, ToggleMorse };

    private static MenuNode options = null;

    private string[] titleScreen = { "New Game", "Chapter Select", "Extras", "Options", "Quit" };
    private string[] soundAdjust = { "Music", "Voices", "Effects", "Morse Code", "Back" };
    private string[] soundLevels = { "Louder", "Quieter", "Back" };
    private string[] rootGameMenu = { "Walk", "Sprint", "Act", "Status", "End Turn", "Menu" };
    private string[] inGameMenu = { "Options", "Quicksave", "Title Screen", "Back" };
    private string[] confirmMenu = { "Confirm", "Cancel" };
    private string[] backMenu = { "Back" };
    private string[] actMenu = { "Firearm", "Frag Grenade", "Smoke Bomb", "Gauze", "Smell Salts", "Back" };
    private string[] extrasMenu = { "Concept Art", "Sound Test", "Film Edit", "Dev Commentary", "Back" };
    private string[] nullMenu = { };

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void MenuCommand()
    {
        
    }

    public static void MapCommand(MenuManager.Context context, Vector2 position)
    {
        // TODO: Replace switch with string instead of enum
        switch (MenuManager.context)
        {
            case MenuManager.Context.MapMove:
                int distance = Grapher.ManhattanDistance(PlayerMover.instance.transform.position, position);
                bool sprinting = MenuNode.GetCurrent() == "Sprint";

                MenuManager.instance.ClearOptions();
                PlayerMover.instance.GetComponent<Navigator>().SetDestination(position, MenuNode.GetCurrent() == "Sprint");
                break;
            case MenuManager.Context.UnitSelect:
                MenuManager.instance.ClearOptions();
                RaycastHit2D hit = Physics2D.Raycast(position, Vector3.zero, 0);
                if (hit.collider == null)
                {
                    Debug.Log("Tried to shoot nonexistent enemy!");
                }
                else
                {
                    PlayerMover.instance.GetComponent<Shooter>().GunAttack(PlayerMover.instance.gameObject, hit.collider.gameObject);
                }
                break;
        }
    }

    public static void BuildTitleMenu()
    {
        MenuNode root = new MenuNode("root", null, MenuNode.NavigationType.Stay);
        root.Add(new MenuNode("New Game", root, MenuNode.NavigationType.Close));

        if(options == null)
            BuildOptionsMenu(root);
        root.Add(options);

        MenuNode.SetCurrent(root);
        root.Command();
    }

    public static void BuildGameMenu()
    {
        MenuNode root = new MenuNode("root", null, MenuNode.NavigationType.Stay);

        MenuNode walk = new MenuNode("Walk", root, MenuNode.NavigationType.Child);
        MenuNode endTurn = new MenuNode("End Turn", root, MenuNode.NavigationType.Close);
        walk.Add(endTurn);
        walk.AddBack();
        root.Add(walk);

        MenuNode sprint = new MenuNode("Sprint", root, MenuNode.NavigationType.Child);
        sprint.Add(endTurn);
        sprint.AddBack();
        root.Add(sprint);

        MenuNode act = new MenuNode("Act", root, MenuNode.NavigationType.Child);
        MenuNode firearm = new MenuNode("Firearm", act, MenuNode.NavigationType.Child);
        firearm.AddBack();
        act.Add(firearm);
        act.AddBack();
        root.Add(act);

        root.Add(new MenuNode("Status", root, MenuNode.NavigationType.Stay));

        root.Add(endTurn);

        if (options == null)
            BuildOptionsMenu(root);
        root.Add(options);

        MenuNode.SetCurrent(root);
        root.Command();
    }

    // Do we want to add zoom in/out options?
    private static void BuildOptionsMenu(MenuNode parent)
    {
        options = new MenuNode("Options", parent, MenuNode.NavigationType.Child);

        options.Add(new MenuNode("Morse Mode", options, MenuNode.NavigationType.Stay));
        options.Add(new MenuNode("Graphics", options, MenuNode.NavigationType.Child));
        MenuNode soundOptions = new MenuNode("Sound", options, MenuNode.NavigationType.Child);
        soundOptions.Add(new MenuNode("Music", soundOptions, MenuNode.NavigationType.Child));
        soundOptions.Add(new MenuNode("Voices", soundOptions, MenuNode.NavigationType.Child));
        soundOptions.Add(new MenuNode("Effects", soundOptions, MenuNode.NavigationType.Child));
        soundOptions.Add(new MenuNode("Morse Code", soundOptions, MenuNode.NavigationType.Child));
        soundOptions.Add(new MenuNode("Back", soundOptions, MenuNode.NavigationType.Parent));
        options.Add(soundOptions);
        options.Add(new MenuNode("Back", options, MenuNode.NavigationType.Parent));
    }

}
