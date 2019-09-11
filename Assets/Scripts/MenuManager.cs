using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/* MenuManager keeps track of the menu options being displayed.
 
AddOption(string name)
    Adds an option to the menu with the provided name. The letter will be randomized.
ClearOptions()
    Clears the menu display.
ShowMenu(string[] menu)
    Shows a menu of the options listed in the array.
MoveMenu(Vector2 start, int radius)
    Spawns letter icons over neighboring tiles on the map.
ClearMenu()
    Clears all reserved letters.
AssignLetter()
    Returns a random character and removes that character from the pool of available ones.
GetSprites()
    Returns Sprite[] of all letter sprite objects.
     
     */

public class MenuManager : MonoBehaviour
{
    public enum Context { Menu, MapMove, UnitSelect };
    public static Context context = Context.Menu;

    public static MenuManager instance;

    public GameObject menuPanel;
    public GameObject menuOption;
    public GameObject letterObject;

    private List<GameObject> currentOptions;

    private Sprite[] sprites;
    //private List<GameObject> litTiles; // selectable map tiles
    private string defaultLetters = "abcdefghijklmnopqrstuvwxyz";
    private List<char> letterBank;
    private List<KeyValuePair<char, string>> options;
    private LayerMask mask = ~(1 << 12);

    private string[] titleScreen = { "New Game", "Chapter Select", "Extras", "Options", "Quit" };
    private string[] soundAdjust = { "Music", "Voices", "Effects", "Morse Code", "Back" };
    private string[] soundLevels = { "Louder", "Quieter", "Back" };
    private string[] rootGameMenu = { "Walk", "Sprint", "Act", "Status", "End Turn", "Menu" };
    private string[] inGameMenu = { "Options", "Quicksave", "Title Screen", "Back" };
    private string[] confirmMenu = { "Confirm", "Cancel" };
    private string[] backMenu = { "Back" };
    private string[] actMenu = { "Firearm", "Frag Grenade", "Smoke Bomb", "Gauze", "Smell Salts", "Back" };
    private string[] extrasMenu = { "Concept Art", "Sound Test", "Film Edit", "Dev Commentary", "Back" };

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        sprites = Resources.LoadAll<Sprite>("Letters"); // Sprites read from Letters folder
        //litTiles = new List<GameObject>();

        currentOptions = new List<GameObject>();
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "TitleScreen")
            CommandManager.BuildTitleMenu();
        else
            CommandManager.BuildGameMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddOption(MenuNode node)
    {
        GameObject newOption = Instantiate(menuOption, transform.position, Quaternion.identity);
        currentOptions.Add(newOption);
        newOption.transform.parent = menuPanel.transform;
        newOption.GetComponent<MenuOption>().SetOption(node, AssignLetter());
    }

    public void ClearOptions()
    {
        letterBank = new List<char>(defaultLetters.ToCharArray());
        for (int i = 0; i < currentOptions.Count; ++i)
            Destroy(currentOptions[i]);
        currentOptions.Clear();
    }

    public void ShowMenu(List<MenuNode> children)
    {
        ClearOptions();
        
        for (int i = 0; i < children.Count; ++i)
            AddOption(children[i]);
    }

    public void MoveMenu(Vector2 startPoint, int radius)
    {
        List<Vector2> tilesInRange = Grapher.instance.MakeGraph(startPoint, radius);

        for (int i = 1; i < tilesInRange.Count; ++i)
        {
            GameObject newLetter = Instantiate(letterObject, tilesInRange[i], Quaternion.identity);
            newLetter.GetComponent<Letter>().SetLetter(AssignLetter());
            currentOptions.Add(newLetter);
        }

    }

    public void TargetMenu(Vector2 startPoint, int radius)
    {
        for(int i = 0; i < EnemyList.instance.transform.childCount; ++i)
        {
            Vector2 direction = (Vector2)EnemyList.instance.transform.GetChild(i).position - startPoint;
            RaycastHit2D hit = Physics2D.Raycast(startPoint, direction, radius, mask);

            if (hit.collider != null && hit.collider.CompareTag("Enemy"))
            {
                GameObject newLetter = Instantiate(letterObject, hit.collider.transform.position, Quaternion.identity);
                newLetter.GetComponent<Letter>().SetLetter(AssignLetter());
                currentOptions.Add(newLetter);
            }
        }
    }

    // Retrieves a random letter, then marks it as unavailable for future use
    public char AssignLetter()
    {
        if (letterBank.Count == 0)
        {
            return 'a';
        }

        int randomIndex = Random.Range(0, letterBank.Count);

        char retVal = letterBank[randomIndex];
        letterBank.RemoveAt(randomIndex);
        return retVal;

    }

    public Sprite[] GetSprites()
    {
        return sprites;
    }

}