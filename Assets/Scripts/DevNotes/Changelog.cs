/*

UX PATCH
 - Detailed unit information during action pause
 - Button labels on rollover/selection
 - Icon/text/both options for buttons
 - Make it clear when player is taking damage
 - Looting popup

ARCADE PATCH
 - End screen for randomized levels (seed/time/damage/money/save)
 - Fix level ending too soon upon collecting loot
 - Don't spawn player in bad position
 - Better distribution of enemies
 - Fewer hiding places

BALANCING PATCH
 - Must be out of sight for 1-3 seconds before knifing target
 - Enemies will ignore corpses if investigating noise/sighting

TUTORIAL PATCH
 - Narrow starting area
 - Message explaining how to move
 - Easy instructions for running, knifing, and shooting
 - Revise tutorial (narrow start, start on movement message)

ARCHITECTURAL PATCH
 - Derive AutoMover and HideMover from same class maybe?
 - Pathfinding to allow diagonal movement (A* maybe)
 - Refactor ActionManager
 - Refactor PanZoom to use ClickManager

FEATURE PATCH
 - I-Troops with tear gas (inhibits sight and accuracy)
 - Hounds with tracking ability
 - Cavalry with fast movement speed
 - Tank boss battle
 - Airship boss battle
 - Unlock levels sequentially
 - Flares to spawn and alert enemies
 - Platform-agnostic controls (menu options and cursor movement)

BUG PATCH
 - Enemies sometimes freeze in Suspicious state
 - Fix enemy friendly fire
 - Limit distance camera can pan from player
 - Grenade can sometimes be thrown infinite distance (probably when aiming farther than max)
 - WoundedEnemy should spawn with red health bar

UNIQUE FEATURES BY CHAPTER
  MERAD - Basic gameplay
  GHENI - Retreating enemies with flares
  CHALT - Cavalry, boss battle
  NENEGI - Hounds, some sequences with only 2 party members, or infinite bombs
  TRIPSI - Mixed evincer/soldier enemies
  REGA - I-Troops
  VENHA - Extremely cramped space

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
 Complete chapter           (1, 2, 3, 4, 5, 6, 7)
 Complete hardmode chapter  (      3, 4,    6, 7)

BONUS FEATURES:
  Beating the game unlocks the following:
    Hardcore Mode - Enemies have no cones; player noises no longer visualized
    Cinema Mode - Autoplay all cutscenes
    Encyclopedia - Appendix of game lore
  Beating the game on hardmode unlocks the following:
    Commentary Mode - Play game with commentary from developers
    Gun Bar Mode - Replaces gun sounds with battle rap voices
  Features unlocked from the beginning:
    Jukebox - Play all game music
    Arcade Mode - Randomly generated maps

 */