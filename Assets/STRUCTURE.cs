/*

GLOBAL
    SoundPlayer
        (Observer pattern to play sound effects)
    GraphMaster
        (Flood fill to build graph)
        getPath(start, end) -> returns list of points

PLAYER
    GridMover
    PlayerMover
    Rotator
           FrontOffset()
                Returns offset between center of object and the edge in the direction it's facing
            GetAngle()
                Returns angle of object
            Rotate(int ang)
                Sets rotation of object
            Rotate(Vector2 ang)
                Sets rotation of object, must have cardinal direction
    Flasher
        (Needs reference to SpriteRenderer)
        Flash(float time)
            Causes object to flash white for time set
        IsFlashing()
            Returns if object is flashing
    Health
        TakeDamage(int dmg = 1)
            Subtracts health, needs reference to a HealthBar object
    SightVisualizer
        UpdateVisualizer()
            Updates visual cone (normally happens on its own but this forces it)
        SetAlert(bool value)
            Sets alert (changes cone color)
 
 
 
 */