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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void UseGauze(int target)
    {
        health[target] = Mathf.Min(3, health[target] + 1);
        numGauze -= 1;
    }

    public static void UseSalts(int target)
    {
        focus[target] = Mathf.Min(3, focus[target] + 1);
        numSalts -= 1;
    }

}
