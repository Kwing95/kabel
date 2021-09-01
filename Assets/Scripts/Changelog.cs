/*
 
October 4, 2019                                             [v0.02]
 + Units have turn-based movement
 + Enemies have fully rendered sight cones
 + Enemies track player using Dijkstra's algorithm
 + Enemies can be notified, evaded, attacked, and killed
 - No generalized sound system
 - Enemies don't have good protocol for attacking player
 - Need to add inventory and usable items
    * Frag, flash, and smoke grenades
    * Gauze and salts
    * Enemy inventory and drops
 - Need more options for AI
    * Stay ideal distance from player
    * Seek cover
    * Become alerted by seeing corpses
 
October 21, 2019                                            [v0.03]
 + Grapher has been optimized
 + Enemies can now properly turn to face the player
 + Enemies attack when they can see the player, chase otherwise
 - Need to add inventory for usable items
 - Need more options for AI
 
September 9, 2020                                           [v0.04]
 + Game now uses "tap-to-navigate" controls
 + Pathfinding now uses semi-straight line for paths in line of sight
 + Can now use buttons to toggle running and camera zoom
 - World clicking counted even when clicking UI buttons

September 25, 2020
 * Grapher contains FindDirectPath and FindIndirectPath; FindPath branches to them
 * FindIndirectPath can use new thread; FindDirectPath can't
 *     Maybe modify FindDirectPath so it can be used in new thread? Eliminate raycast?
 * Only Navigator can start a new thread, as it knows what to do with the path it generates
 * Grapher can have DirectLine which Navigator accesses; this way Navigator can make a single call
 
October 16, 2020
 + Can now navigate menus and preview gun attack

October 17, 2020
 + Can now use gun attack, buttons are now toggled as needed for actions and zooming
 - Confirm button works even if no target is selected
 - Unpausing game enables zoom buttons even if they shouldn't be
 - Enemies sometimes run into walls

October 21, 2020
 + Show player's destination while moving

 October 22, 2020
 + Enemy lock-on improved
 + Health indicator added
 + Gunshot graphic looks better

October 24, 2020
 + Placeholders replaced with figures
 + Shooter completely phased out
 + Action button properly disabled during pause

October 27, 2020
 + Can now select "escape" destination before attack occurs
 + Player can always set escape location
 + Enemies can now investigate Noise object at non-integer position

November 2, 2020
 + Navigators no longer go through walls
 + Enemies now show white/yellow/red depending on alert status
 + Enemies now track properly; sight cone locks on when spotted
 + Enemies will not chase player if too close

November 4, 2020
 + Enemy lock-on persists when enemy is stationary

November 8, 2020
 + Grenade preview shows area of effect and units affected

November 12, 2020
 + Bounding boxes now function as health indicators
 + Enemies now have "corner" boxes; player has solid box

February 2, 2021
 + Enemies should lock onto correct spot

February 5, 2021
 + Enemies have confused lookaround
 + Enemies have randomPatrol

February 9, 2021
 + Enemies beccome more territorial with leashLength

February 11, 2021
 + Enemies may add up to [pointsMemory] points to a patrol
 - Enemies sometimes freeze in Suspicious when chasing (UNKNOWN)

February 21, 2021
 + Most buttons permanently docked on right side of screen
 + Mask to hide things outside player's field of view; augmented camera zoom levels
 + Player can set destination during attack phase
 + All lbuttons except pause hidden during true pause

February 25, 2021
 + Refactored zoom code to accomodate PanZoom
 + ClickManager differentiates press and release now
 + Can pan around map, snaps again when player moves
 + Pinch zoom implemented for mobile
 + Zoomer and Follower can pause depending on PanZoom state
 + Interactions trigger on release instead of on press

March 1, 2021
 + Refactored some ActionManager code
 + Can no longer drag and return camera to release

March 8, 2021
 + Incomplete grenade implementation

March 10, 2021
 + Grenade rolling animation
 + Noise now has visual effects moved to Fader and Expander

March 14, 2021
 + Grenade damage fixed
 + Fixed issues with UI clicks registered as world clicks

March 21, 2021
 + Units can interact with tear gas clouds

May 3, 2021
 + Knife implemented

May 4, 2021
 + Fixes to AI freeze: Set idle to true and pause to false, ensured extraPoints were integers
 + Fixes to AI stopping: If enemy can't see player's NEXT point, investigate that point

June 12, 2021
 + AI can think without freezing game

July 5, 2021
 + Can use pause menu to navigate between scenes

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

 - Enemies may freeze while investigating corpse
 - Save is not create
 - Sometimes bad health color indicator
 - Platform-agnostic controls
 - Looting popup

 - Refactor ActionManager
 - AI sometimes walks through/into walls
 - AI sometimes freezes during yellow patrol
 - Parallax foreground 3D walls
 - Reproduce AI freeze: Let enemy see you at close range, walk around corner. Note navigator idle bool is false
    Point memory? Doesn't look like it, problem remains with pointMemory = 0
    pausePathFinding sometimes true, path count sometimes 0
    path count 0 when destination is non-integer
    pausePathFinding stuck to true if LookAround is called twice?
    navigator wouldn't set destination because destination was a duplicate, so Pause(false) was never run

 - G1-1:
     HOW TO: Move, run, change camera, knife
     FEATURES: Some small obstacles, one stationary enemy (for knifing)
     RETREAT: 3 advancing enemies, 3 patrolling enemies
 - G1-2:
     HOW TO: Avoid enemies, distract, shoot
     FEATURES: Medium obstacles, 2 moving enemies (for avoiding,) 1 stationary enemy (distract or shoot) inside outpost
     RETREAT: 2 advancing enemies, 2 stationary enemies, 1 patrolling enemy
 - G1-3:
     HOW TO: Deal with multiple enemies, grenade
     FEATURES: 2 enemies to avoid, 3 enemies to grenade
     RETREAT: 1 advancing enemy, 3 stationary enemies

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

Enemy behavior: Enemies pause in confusion after losing player
                Enemies may pause after reaching waypoint in route
                Enemies respond to seeing dead bodies

Distractable: Enemies may follow other alerted enemies or remain
              at their post.

Choke points: Upon an alarm being raised, enemies will rush
              to defend choke points to halt player advance. Which
              enemy defends which choke points is a race condition.

Gunshot can be heard from 1 mile (~5000ft) away; silenced gun from 600ft
Conservative estimate could be make gunshots audible for enemies 60 tiles away

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