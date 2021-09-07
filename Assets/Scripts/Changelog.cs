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

 - AutoMover Coroutine glitch
 - Menu navigation to bring up levels
 - Save file to include level completions
 - Retreating enemies
 - Sometimes bad health color indicator
 - Platform-agnostic controls
 - Looting popup

 - Refactor ActionManager
 - AI sometimes walks through/into walls
 - AI sometimes freezes during yellow patrol
 - Parallax foreground 3D walls

 - Tear gas was invented in 20s; rubber bullets, mace, and stun grenades not until 60s and later
 - Grenade can sometimes be thrown infinite distance (probably when aiming farther than max)

 - Enable console/keyboard controls
    D-pad to move           X to act/confirm        O to cancel/run     L1/R1 to zoom
    Start to pause          (R) move camera

    WASD/Arrows move        Spacebar to act         BKSPC cancel    Q/E to zoom
    Shift to run            ESC to pause            Ctrl + Arrows to pan camera

 - Possibly refactor PanZoom to use ClickManager, refactor ClickManager for general use?

 - Let enemies "take cover"; find area with no line of sight to player
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

 */