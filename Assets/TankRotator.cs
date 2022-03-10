using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TankRotator is to be attached to the TANK SPRITE
public class TankRotator : Rotator
{

    public AutoMover autoMover;
    public Rotator turretRotator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void FixedUpdate()
    {
        if (lockedOnTarget)
        {
            destinationAngle = (int)Vector2.SignedAngle(Vector2.up, lockOnPosition - (Vector2)transform.position);
        }

        currentAngle = Quaternion.Lerp(Quaternion.Euler(0, 0, currentAngle), Quaternion.Euler(0, 0, destinationAngle), Time.deltaTime * 10).eulerAngles.z;

        bool isIdle = autoMover.GetAwareness() == AutoMover.State.Idle;

        // Sprite
        if(subjects.Count > 0 && subjects[0])
        {
            turretRotator.enabled = true;
            //autoMover.rotator = turretRotator;
            subjects[0].transform.rotation = Quaternion.Euler(Vector3.forward * currentAngle);
        }

        // Turret
        if (subjects.Count > 1 && subjects[1] && isIdle)
        {
            turretRotator.enabled = false;
            //autoMover.rotator = this;
            subjects[1].transform.rotation = Quaternion.Euler(Vector3.forward * currentAngle);
        }
            

    }

    /*public override void Rotate(int ang)
    {
        // If locked onto a specific target, don't look in a different direction
        if (lockedOnTarget)
            return;

        destinationAngle = mod(ang, 360);
        //sprite.transform.rotation = Quaternion.Euler(Vector3.forward * currentAngle);
    }*/
}
