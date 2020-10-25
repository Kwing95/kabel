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
 - Actions other than Gun not implemented
 - Navigators sometimes go into walls
 - Enemies should stop at a distance from player
 - Pathfinding should include hybrid of Dijkstra's and direct
 - Cooldown and movement based on health (-1 HP = -1 move, -1 focus = +1 act cooldown)
 - Limit ammunition
 - Limit player's field of view
 - Allow player to select escape route after acting

 */

/*

FEATURES TO ADD:

Flushing: Enemy will check all points within N tiles of the spot
          the player was last heard. A point is checked if it
          is within the direct line of sight of the enemy while
          performing a flushing operation.

Coroutine pathfinding: When paths are generated, enemies freeze
                       instead of game freezing; pathfinding is
                       computed inside of a coroutine.

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

https://docs.unity3d.com/ScriptReference/Resources.Load.html
https://answers.unity.com/questions/917138/text-file-not-loading-after-building-the-game.html

 */