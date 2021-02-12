using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals : MonoBehaviour
{

    // PREFABS
    public static readonly GameObject CORPSE = Resources.Load<GameObject>("Prefabs/Core_Gameplay/Corpse");
    public static readonly GameObject NOISE = Resources.Load<GameObject>("Prefabs/Core_Gameplay/Noise");
    public static readonly GameObject CURSOR = Resources.Load<GameObject>("Prefabs/UI/Cursor");
    public static readonly GameObject WAYPOINT = Resources.Load<GameObject>("Prefabs/UI/Waypoint");

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
