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

CANCELLED CONTENT
 * Flushing: Enemies systematically search all tiles in a certain area
 * Interception: Alerted enemies guard critical areas of map

 */