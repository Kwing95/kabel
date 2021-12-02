/*

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

Nov 10, 2021
 + Random map now has a few walls missing
 + Random mazes now have islands and deformities

Nov 11, 2021
 + Reorganized Scripts folder

Nov 19, 2021
 + Can spawn enemies with routes

Nov 21, 2021
 + Can spawn and collect loot
 + Organized script files into UI folder

Nov 23, 2021
 + Fixed player and enemy default position bug in random maps
 + Random maps can be replicated with seeding number

Dec 1, 2021
 + Worked on fixing bad pathfinding
   + FindDirectPath can add duplicate squares; created check to 
   + pathProgress sometimes doesn't index adjacent tile; can reset to 0 or 1

SCRIPT EXECUTION ORDER
 - MazeMaker: Must run before Grapher because it modifies the map
 - Grapher: Must run after MazeMaker so that it can graph an up-to-date map
 - PlayerMover: Must run before other scripts that need reference to PlayerMover.instance
 - GridMover: Must Awake to initialize "rotator"
 - Health: 

DEBUGGING
 - Enemies walk through walls when the direction argument of ChangeDirection is not a cardinal. This happens
    when their current position is not adjacent to the path
 - [Seed 3, enemyCount 1] Navigator path contains same position twice; direction will be 0,0
 - [Seed 4, enemyCount 3] 

 - Enemy routes may go through walls (Globals should initialize afterward? Race condition?)
 - Make levels repeatable (seeding number)

BALANCING
 - Increased distance player can indirectly move (maybe path to raycast, then Dijkstra's?)
 - Detailed unit health data during "dimmed" action pause
 - Revise tutorial (narrow start, start on movement message, make knifing easier)
 - Disable non-essential buttons (chapter)
 - Make distract/knife easier

 - Make it clear when player is taking damage
 - Use multithreading for HideMover
 - Save file to unlock next level
 - Platform-agnostic controls (menu options and cursor movement)
 - Looting popup

 - Refactor ActionManager
 - AI sometimes walks through/into walls
 - AI sometimes freezes during yellow patrol
 - Parallax foreground 3D walls

 - Gas inhibits sight distance, firing accuracy, and action cooldown

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

 - Enemies can probably hit overlapping enemies (make gun start projected one tile out)
 - Pathfinding should include hybrid of Dijkstra's and direct
 - Figure out health system
 - Stationary enemies have "leashLength" so they guard certain points
 - Implement player stun (pauses action cooldown AND movement) (?)

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