using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMover : AutoMover
{

    public Rotator spriteRotator;

    // If enemy is Idle, line of sight defaults to rotation of the tank sprite
    // If enemy is Alert, line of sight follows turret (default rotator,) which moves independently of tank sprite
    protected override bool CanSeePoint(Vector2 point)
    {
        // Check that player is within view angle
        Vector2 direction = point - (Vector2)transform.position;

        Rotator usedRotator = GetAwareness() == State.Idle ? spriteRotator : rotator;
        float angleToPoint = Mathf.Abs(Vector2.SignedAngle(direction, usedRotator.GetCurrentAngleVector()));

        return angleToPoint < fieldOfView.viewAngle / 2 && ClearView(point) && Vector2.Distance(point, transform.position) <= sightDistance;
    }
}
