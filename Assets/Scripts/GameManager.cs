using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    // Bane, Biggs, Saito, Wedge
    // Asra, Diego, Thane
    // Diego, Jacques, Thane

    public static int actionPoints = 5;
    public static bool running = false;

    public static int[] health = { 3, 3, 3 };
    public static int[] focus = { 3, 3, 3 };

    private static int numGauze = 3;
    private static int numSalts = 3;
    public static int numFrag = 0;
    public static int numSmoke = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}