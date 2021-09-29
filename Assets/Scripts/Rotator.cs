using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Rotates one or more objects, either manually (set rotation) or automatically (lock onto position) */

public class Rotator : MonoBehaviour
{

    public List<GameObject> subjects;

    private int destinationAngle = 0;
    private float currentAngle = 90;
    private bool lockedOnTarget = false;
    private Vector2 lockOnPosition;

    // Start is called before the first frame update
    void Awake()
    {
        destinationAngle = Mathf.RoundToInt(transform.rotation.eulerAngles.z);
        Rotate(destinationAngle);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (lockedOnTarget)
        {
            destinationAngle = (int)Vector2.SignedAngle(Vector2.up, lockOnPosition - (Vector2)transform.position);
        }

        currentAngle = Quaternion.Lerp(Quaternion.Euler(0, 0, currentAngle), Quaternion.Euler(0, 0, destinationAngle), Time.deltaTime * 10).eulerAngles.z;

        foreach (GameObject elt in subjects)
            elt.transform.rotation = Quaternion.Euler(Vector3.forward * currentAngle);
            
    }

    // Maybe put FaceDirection inside its own Rotator class?

    public void Rotate(int ang)
    {
        // If locked onto a specific target, don't look in a different direction
        if (lockedOnTarget)
            return;

        destinationAngle = mod(ang, 360);
        //sprite.transform.rotation = Quaternion.Euler(Vector3.forward * currentAngle);
    }

    public static int mod(int x, int m)
    {
        int r = x % m;
        return r < 0 ? r + m : r;
    }

    public Vector2 GetCurrentAngleVector()
    {
        return Quaternion.Euler(0, 0, currentAngle) * Vector2.up;
    }

    // Binds angle within [0, 360] range
    public static int SignedAngle(float angle)
    {
        return mod((int)-angle, 360);
    }

    public void FacePoint(Vector2 point)
    {
        float rawAngle = Vector2.SignedAngle(Vector2.up, point - (Vector2)transform.position);
        int cardinalAngle = Mathf.RoundToInt(rawAngle / 90) * 90;

        Rotate(cardinalAngle);
    }

    // Returns the offset of the tile relative to the direction it's facing
    public Vector2 FrontOffset()
    {
        switch (currentAngle)
        {
            case 0:
                return Vector2.up / 2;
            case 180:
                return Vector2.down / 2;
            case 90:
                return Vector2.left / 2;
            case 270:
                return Vector2.right / 2;
        }
        return Vector2.zero;
    }

    // TODO: Figure out when current angle is needed instead
    public int GetDestinationAngle()
    {
        return destinationAngle;
    }

    public float GetCurrentAngle()
    {
        return currentAngle;
    }

    public void ToggleLock(bool isLocked, Vector2 position)
    {
        lockedOnTarget = isLocked;
        lockOnPosition = position;
    }

}
