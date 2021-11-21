using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideMover : MonoBehaviour
{

    public int maximumFleeDistance;
    private Navigator navigator;

    // Start is called before the first frame update
    void Start()
    {
        navigator = GetComponent<Navigator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ClearView(navigator.GetDestination(), PlayerMover.instance.transform.position))
        {
            //Grapher.NativeHide(transform.position, PlayerMover.instance.transform.position);
            navigator.Hide(PlayerMover.instance.transform.position);
            //navigator.path
            //navigator.SetDestination()
        }
    }

    private bool ClearView(Vector2 pointA, Vector2 pointB)
    {
        // Don't look farther than distance of point of interest, but don't look farther than sightDistance either
        LayerMask mask = LayerMask.GetMask(new string[] { "Default", "Player" });
        float effectiveDistance = Mathf.Min(Vector2.Distance(pointA, pointB));
        RaycastHit2D hit = Physics2D.Raycast(pointA, pointB - pointA, effectiveDistance, mask);

        bool returnValue = hit.collider == null || hit.collider.CompareTag("Player");
        return returnValue;
    }
}