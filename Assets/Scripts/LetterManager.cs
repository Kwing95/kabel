using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* LetterManager keeps track of which letters are being used.
 
MoveMenu(Vector2 start, int radius)
    Spawns letter icons over neighboring tiles on the map.
ClearMenu()
    Clears all reserved letters.
AssignLetter()
    Returns a random character and removes that character from the pool of available ones.
GetSprites()
    Returns Sprite[] of all letter sprite objects.
     
     */

public class LetterManager : MonoBehaviour
{

    public static LetterManager instance;

    public delegate Vector2 Vector2Char(char c);
    public static Vector2Char onMapCommand;

    public GameObject letterObject;

    private Sprite[] sprites;
    private List<GameObject> litTiles;
    private string defaultLetters = "abcdefghijklmnopqrstuvwxyz";
    private List<char> letterArray;
    private List<KeyValuePair<char, string>> options;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        sprites = Resources.LoadAll<Sprite>("Letters");
        litTiles = new List<GameObject>();
        ClearMenu();

        // MoveMenu(new Vector2(-1, 2), 3);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveMenu(Vector2 startPoint, int radius)
    {
        ClearMenu();

        List<Vector2> tilesInRange = Grapher.instance.Diamond(startPoint, radius);

        for (int i = 1; i < tilesInRange.Count; ++i)
        {
            GameObject newLetter = Instantiate(letterObject, tilesInRange[i], Quaternion.identity);
            newLetter.GetComponent<Letter>().SetLetter(AssignLetter());
            litTiles.Add(newLetter);
        }
    }

    public void ClearMenu()
    {
        letterArray = new List<char>(defaultLetters.ToCharArray());
        for (int i = 0; i < litTiles.Count; ++i)
            Destroy(litTiles[i]);
    }

    // Retrieves a random letter, then marks it as unavailable for future use
    public char AssignLetter()
    {
        if(letterArray.Count == 0)
        {
            return 'a';
        }

        int randomIndex = Random.Range(0, letterArray.Count);

        char retVal = letterArray[randomIndex];
        letterArray.RemoveAt(randomIndex);
        return retVal;
        
    }

    public Sprite[] GetSprites()
    {
        return sprites;
    }

}