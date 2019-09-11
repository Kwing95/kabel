using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuNode
{
    public char character;

    private static MenuNode current;
    private MenuNode parent;
    private List<MenuNode> children;

    private string itemName;
    private CommandManager.MenuFunction functionId;

    public MenuNode()
    {
        children = new List<MenuNode>();
    }

    public MenuNode(string _itemName, MenuNode _parent, CommandManager.MenuFunction _functionId)
    {
        children = new List<MenuNode>();

        itemName = _itemName;
        parent = _parent == null ? this : _parent; // Node defaults to being its own parent
        functionId = _functionId;
    }

    public void Command()
    {
        // If a menu item has children, selecting it should open up its submenu
        switch (functionId)
        {
            case CommandManager.MenuFunction.None:
                MenuManager.instance.ShowMenu(children);
                current = this;
                break;
            case CommandManager.MenuFunction.Back:
                current = parent.parent;
                MenuManager.instance.ShowMenu(current.children);
                break;
            case CommandManager.MenuFunction.Walk:
                MenuManager.context = MenuManager.Context.MapMove;
                MenuManager.instance.ShowMenu(children);
                current = this;
                // NOTE: Need to get AP from Navigator component; Navigator needs to handle AP
                MenuManager.instance.MoveMenu(PlayerMover.instance.transform.position, Mathf.FloorToInt(PlayerMover.instance.GetComponent<FieldUnit>().GetAP() / 2));
                break;
            case CommandManager.MenuFunction.Sprint:
                MenuManager.context = MenuManager.Context.MapMove;
                MenuManager.instance.ShowMenu(children);
                current = this;
                MenuManager.instance.MoveMenu(PlayerMover.instance.transform.position, Mathf.Min(3, PlayerMover.instance.GetComponent<FieldUnit>().GetAP()));
                break;
            case CommandManager.MenuFunction.Target:
                MenuManager.context = MenuManager.Context.UnitSelect;
                MenuManager.instance.ShowMenu(children);
                current = this;
                MenuManager.instance.TargetMenu(PlayerMover.instance.transform.position, 10);
                break;
            case CommandManager.MenuFunction.EndTurn:
                CommandManager.EndTurn();
                break;
        }

    }

    public void Add(MenuNode newNode)
    {
        children.Add(newNode);
    }

    public void AddBack()
    {
        children.Add(new MenuNode("Back", this, CommandManager.MenuFunction.Back));
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

public class CommandManager : MonoBehaviour
{

    // Use "None" if option only navigates menu
    public enum MenuFunction { None, NewGame, Options, Back, Walk, Sprint, Target, EndTurn };

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

    public static void EndTurn()
    {
        for (int i = 0; i < EnemyList.instance.transform.childCount; ++i)
        {
            EnemyList.SetEnemiesMoving(true);
            EnemyList.instance.transform.GetChild(i).GetComponent<FieldUnit>().ResetAP();
        }
        MenuManager.instance.ClearOptions();
    }

    public static void BuildTitleMenu()
    {
        MenuNode root = new MenuNode("root", null, MenuFunction.None);
        root.Add(new MenuNode("New Game", root, MenuFunction.NewGame));

        if(options == null)
            BuildOptionsMenu(root);
        root.Add(options);
        
        root.Command();
    }

    public static void BuildGameMenu()
    {
        MenuNode root = new MenuNode("root", null, MenuFunction.None);

        MenuNode walk = new MenuNode("Walk", root, MenuFunction.Walk);
        MenuNode endTurn = new MenuNode("End Turn", root, MenuFunction.EndTurn);
        walk.Add(endTurn);
        walk.AddBack();
        root.Add(walk);

        MenuNode sprint = new MenuNode("Sprint", root, MenuFunction.Sprint);
        sprint.Add(endTurn);
        sprint.AddBack();
        root.Add(sprint);

        MenuNode act = new MenuNode("Act", root, MenuFunction.None);
        MenuNode firearm = new MenuNode("Firearm", act, MenuFunction.Target);
        firearm.AddBack();
        act.Add(firearm);
        act.AddBack();
        root.Add(act);

        root.Add(endTurn);

        if (options == null)
            BuildOptionsMenu(root);
        root.Add(options);
        
        root.Command();
    }

    private static void BuildOptionsMenu(MenuNode parent)
    {
        options = new MenuNode("Options", parent, MenuFunction.None);
        options.Add(new MenuNode("Graphics", options, MenuFunction.None));
        MenuNode soundOptions = new MenuNode("Sound", options, MenuFunction.None);
        soundOptions.Add(new MenuNode("Music", soundOptions, MenuFunction.None));
        soundOptions.Add(new MenuNode("Voices", soundOptions, MenuFunction.None));
        soundOptions.Add(new MenuNode("Effects", soundOptions, MenuFunction.None));
        soundOptions.Add(new MenuNode("Morse Code", soundOptions, MenuFunction.None));
        soundOptions.Add(new MenuNode("Back", soundOptions, MenuFunction.Back));
        options.Add(soundOptions);
        options.Add(new MenuNode("Back", options, MenuFunction.Back));
    }

}
