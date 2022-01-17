/*
 
PATROL

Enemies with 0-1 waypoints guard a single position
    Enemies guarding a position may have a "leash" preventing them from wandering too far from their objective
        Enemies with a leash will chase the player to the end of their leash, then guard that position for a time
        before returning to their primary objective

Enemies with 2+ waypoints move between waypoints
    These may be visited in random order, or sequentially (either back and forth, or a circuit)
        If the player is spotted, enemies with a random patrol may add the most recent N tiles where the player
        was spotted, revisiting locations where the player was known to have been

ALERTS

Enemies react differently to different sounds
    They inspect the source of the player's gunfire, footsteps, or knife attack
    If damaged by a grenade, enemies inspect the location it was thrown from
        If the blast is heard from a distance, the source of the grenade is inspected instead
    They inspect the tile their ally is inspecting if their ally makes noise (so as to investigate together)
    They inspect the source of a player's distraction, but slowly
        If the enemy previously spotted the player or a downed ally, they will sprint toward the sound

Upon finding a downed ally, enemies will shoot a flare gun to summon 2 allies

Upon returning to a patrol, enemies will shoot a flare gun to summon 1 ally
    If the enemy took damage, 2 allies will be summoned

After inspecting an area, enemies look in three random directions before returning to their patrol

DEATH

Wounded enemies run to nearest tile not in direct view of player
Wounded enemies may shoot a flare gun to summon 2 allies

AUTOMOVER RESPONSIBILITIES
 - Patrol
   - Idle patrol
   - Investigation
   - Chasing
   - Revising patrols (remembering points, leash)
 - Attacking
 - Awareness
   - Look and listen
   - Inspect corpses
 - Confuse (LookAround)

 */