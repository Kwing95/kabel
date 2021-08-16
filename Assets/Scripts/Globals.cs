using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals : MonoBehaviour
{
    // DATA
    // public static readonly TextAsset LEVEL_DATA = Resources.Load<TextAsset>("levelList");
    // public static readonly TextAsset SAVE_DATA = Resources.Load<TextAsset>("saveData");
    public static readonly TextAsset GAME_SCRIPT = Resources.Load<TextAsset>("gameScript");
    public static readonly Dictionary<string, LevelData> LEVEL_DATA = new Dictionary<string, LevelData> {
        // {"1-1", new LevelData(sceneTitle, isPlayable, sceneName, hasHardmode) },
        {"S1-1", new LevelData("The Classroom", false, "S1-1", false) },
        {"S1-2", new LevelData("Horseback", false, "S1-2", false) },
        {"G1-1", new LevelData("Infiltration", true, "G1-1", false) }
    };
    public static readonly List<string> AUTOPLAY_LIST = new List<string> { "S1-1", "S1-2", "G1-1", "G1-2", "G1-3", "S1-3",
        "S1-4", "G1-4", "G1-5", "G1-6", "S1-5", "S1-6" };
    public static readonly List<string> CINEMA_LIST = new List<string> { "S1-1", "S1-2", "S1-3", "S1-4", "S1-5", "S1-6" };

    // CONSTANTS
    public static readonly int INFINITY = 1073741823;
    public static readonly int NUM_CHAPTERS = 7;
    public static readonly int NUM_LEVELS = 100;
    public static readonly float EPSILON = 0.01f;

    public static readonly float GRENADE_VOLUME = 10;
    public static readonly float GUN_VOLUME = 10;
    public static readonly float KNIFE_VOLUME = 5;
    public static readonly float GAS_VOLUME = 5;
    public static readonly float DISTRACTION_VOLUME = 5;
    public static readonly float RUN_VOLUME = 3.5f;

    // PREFABS
    public static readonly GameObject CORPSE = Resources.Load<GameObject>("Prefabs/Core_Gameplay/Corpse");
    public static readonly GameObject EXPLOSION = Resources.Load<GameObject>("Prefabs/Core_Gameplay/Explosion");
    public static readonly GameObject NOISE = Resources.Load<GameObject>("Prefabs/Core_Gameplay/Noise");
    public static readonly GameObject PROJECTILE = Resources.Load<GameObject>("Prefabs/Core_Gameplay/Projectile");
    public static readonly GameObject CURSOR = Resources.Load<GameObject>("Prefabs/UI/Cursor");
    public static readonly GameObject WAYPOINT = Resources.Load<GameObject>("Prefabs/UI/Waypoint");
    public static readonly GameObject GAS_CLOUD = Resources.Load<GameObject>("Prefabs/Core_Gameplay/Gas_Cloud");
    public static readonly GameObject WALL = Resources.Load<GameObject>("Prefabs/Environment/Wall");

    // MATERIALS
    public static readonly Material WHITE = Resources.Load<Material>("Materials/White");
    public static readonly Material BRIGHT_WHITE = Resources.Load<Material>("Materials/Bright_White");
    public static readonly Material RED = Resources.Load<Material>("Materials/Red");
    public static readonly Material BRIGHT_RED = Resources.Load<Material>("Materials/Bright_Red");
    public static readonly Material ORANGE = Resources.Load<Material>("Materials/Orange");
    public static readonly Material YELLOW = Resources.Load<Material>("Materials/Yellow");
    public static readonly Material BRIGHT_YELLOW = Resources.Load<Material>("Materials/Bright_Yellow");
    public static readonly Material BRIGHT_BLUE = Resources.Load<Material>("Materials/Bright_Blue");

    // SPRITES
    public static readonly Sprite RUN_ICON = Resources.Load<Sprite>("Graphics/menu_buttons_6");
    public static readonly Sprite WALK_ICON = Resources.Load<Sprite>("Graphics/menu_buttons_5");
    public static readonly Sprite PAUSE_ICON = Resources.Load<Sprite>("Graphics/menu_buttons_1");
    public static readonly Sprite UNPAUSE_ICON = Resources.Load<Sprite>("Graphics/menu_buttons_0");
}
