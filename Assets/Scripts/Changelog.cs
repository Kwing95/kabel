/*

Aug 10, 2021
 + Working dialogue parser for scenes and in-game prompts
 + Partial implementation for 3D walls
 + Implemented Distract action

Aug 15, 2021
 + Prototypes of G1-1 through G1-3
 + Radial action button refresh

Aug 21, 2021
 + Enemy AI is smarter responding to grenades
 + Framework for specific behavior regarding sight/sound

Aug 29, 2021
 + Enemy -> corpse -> player inventory

Aug 29, 2021
 + Enemies investigate corpses
 + Medkits are enabled

Aug 31, 2021
 + Save is created
 + All scenes use new ObjectContainer

Sep 1, 2021
 + Enemies no longer freeze from corpses

Sep 5, 2021
 + Level Selector can generate buttons from List<LevelData> and infer level data

Sep 6, 2021
 + Cutscene parsing fixed
 + Inventory system handles wallet

Sep 7, 2021
 + Game saves (time, wallet, damageTaken) values
 + Created HideMover which seeks cover from player

Sep 8, 2021
 + Enemy confuse runs on timer instead of coroutine

Sep 13, 2021
 + Sidebar uses updated button styling
 + Fixed bad health indicator
 + Completion time now rounds to 2 decimal places

Sep 16, 2021
 + Can navigate from title screen to level select, placeholders for Extras and Options menus

Sep 19, 2021
 + Can navigate to TextCutscene scene to read script if no scene exists

Sep 25, 2021
 + Cutscenes are now loaded from gameScript.txt instead of hardcoded into Inspector
 + Added LevelData in Globals for Chapter 2 and some of Chapter 3

Sep 26, 2021
 + Inventory determines whether or not player can use an item
 + Items are consumed when used

Sep 27, 2021
 + Player gun is more accurate
 + Player line of sight limited by blindfold object
 + Retry menu appears when player dies

Sep 28, 2021
 + Fixed glitch where gun aim was off when attacking before moving
 + Can restart/skip/quit from cutscenes
 + Can view cutscenes without dedicated scenes

Oct 8, 2021
 + Balanced game based on scarcity
 + Partial work on refactoring ActionManager

Oct 20, 2021
 + Refactored ActionManager, PreviewManager, and Projectile

Oct 21, 2021
 + Grenades no longer damage through walls
 + Deleted commented code in ActionManager
 + Cannot use knife without a target
 + Walking generates noise
 + Enemy no longer detects player by walking past them
 + Enemies should not damage themselves
 + Fixed aiming bug from PreviewManager refactor

Oct 22, 2021
 + Enemy grenades work

Oct 25, 2021
 + Back button disabled when at beginning of cutscene
 + Screen dims in Action Menu
 + Camera following always pauses in action menu now
 + Enemy friendly fire should be disabled

Nov 8, 2021
 + RandomMap creates random mazes

 - Make levels repeatable (seeding number)
 - Make RandomMap smaller (50x50 maybe)
 - Remove additional walls
 - Add additional wall tiles inside maze without blocking paths

 - Increased distance player can indirectly move
 - Detailed unit health data during "dimmed" action pause
 - Revise tutorial (narrow start, start on movement message, make knifing easier)
 - Disable non-essential buttons (chapter)
 - Make distract/knife easier

 - Make it clear when player is taking damage
 - Arcade mode
 - Use multithreading for HideMover
 - Save file to unlock next level
 - Platform-agnostic controls (menu options and cursor movement)
 - Looting popup

 - Refactor ActionManager
 - AI sometimes walks through/into walls
 - AI sometimes freezes during yellow patrol
 - Parallax foreground 3D walls

 - Tear gas was invented in 20s; rubber bullets, mace, and stun grenades not until 60s and later
 - Grenade can sometimes be thrown infinite distance (probably when aiming farther than max)

IN-GAME PAUSE MENU
  Resume
  Options
    Audio
    Graphics
  Quit

MAIN MENU
  Play
  Extras
    Cinema Mode
    Encyclopedia
    Jukebox
    Dev Commentary
    Gun Bar Mode
    Arcade Mode
  Options
  Quit
  

 - Enable console/keyboard controls
                    MOBILE          DESKTOP         CONSOLE
    MOVEMENT        Tap             WASD            Directional pad
    SPRINTING       Sprint button   LShift          Hold O
    PAUSE           Pause button    Escape key      Start button
    CAMERA          Tap and drag    Ctrl + WASD     Right stick
    ACTION MENU     Action button   Spacebar        X
    MENU CANCEL     Back button     LShift          O
    ZOOM            Pinch           Q / E keys      L / R triggers

 - Possibly refactor PanZoom to use ClickManager, refactor ClickManager for general use?

 - Touching enemies is weird (not important if sound is added for walking too)
 - Enemies can probably hit overlapping enemies (make gun start projected one tile out)
 - Pathfinding should include hybrid of Dijkstra's and direct
 - Figure out health system
 - Cooldown and movement based on health (-1 HP = -1 move, -1 focus = +1 act cooldown)
   - Replace Focus with status ailment? Reduce movement/increase cooldown?
 - Limit ammunition
 - Limit player's field of view
 - Stationary enemies have "leashLength" so they guard certain points
 - Implement player stun (pauses action cooldown AND movement)

 */

/*

FEATURES TO ADD:

Flushing: Enemy will check all points within N tiles of the spot
          the player was last heard. A point is checked if it
          is within the direct line of sight of the enemy while
          performing a flushing operation.

Backup: Enemies will add corpse position to patrol circuit, call in
        backup to patrol area by corpse, or both.

Gunshot can be heard from 1 mile (~5000ft) away; silenced gun from 600ft
Conservative estimate could be make gunshots audible for enemies 60 tiles away

Erawan: Throws flat bombs (grenades)
i-Troops: Throws tear gas
Horses: Faster, more susceptible to stun grenades
Dogs: Tracks player after picking up their scent
Vollmer: Fast and invulnerable to damage
Airship: Flies over everything, constantly fires at player

Firearm: Deals damage to single target
Rock: Makes noise when thrown
Frag Grenade: Deals damage to surrounding area
Stun Grenade: Renders targets unable to act for some time
Smoke Grenade: A cloud that blocks enemy vision
Gauze: Restores health and cures limping status

ACHIEVEMENTS:
 Complete chapter (1, 2, 3, 4, 5, 6, 7)
 Complete hardmode chapter (3, 4, 6, 7)

BONUS FEATURES:
 Hardcore Mode - Enemies have no cones; player's line of sight is limited
 Cinema Mode - Autoplay all cutscenes
 Encyclopedia - Appendix of game lore
 Jukebox - Play all game music
 Commentary Mode - Play game with commentary from developers
 Gun Bar Mode - Replaces gun sounds with battle rap voices
 Arcade Mode - Randomly generated maps

 */