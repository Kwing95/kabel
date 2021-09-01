using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasCloud : MonoBehaviour
{
    public static float cloudRadius = 3.5f;
    public static float updateFrequency = 1;
    private float updateCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        updateCounter += Time.deltaTime;
        if(updateCounter >= updateFrequency)
        {
            UpdateAffectedUnits();
            updateCounter = 0;
        }
    }

    private void UpdateAffectedUnits()
    {
        List<GameObject> units = ObjectContainer.GetAllUnits();

        // Check for all units
        foreach (GameObject elt in units)
        {
            float sourceToTarget = Vector2.Distance(transform.position, elt.transform.position);
            Gassable target = elt.GetComponent<Gassable>();

            // Can only be gassed if target has Gassable component and is within range
            if (target && sourceToTarget <= cloudRadius)
            {
                LayerMask mask = LayerMask.GetMask("Default");
                RaycastHit2D hit = Physics2D.Raycast(transform.position, target.transform.position - transform.position, cloudRadius, mask);
                // If raycast collides with something closer than target, target was shielded
                if (hit.collider && Vector2.Distance(transform.position, hit.point) < sourceToTarget)
                    continue;

                target.SetGassed(true);
            }
        }
    }
}
