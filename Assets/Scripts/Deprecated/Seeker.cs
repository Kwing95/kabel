using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seeker : MonoBehaviour
{

    //public List<GameObject> subjects;
    private Vector2 location;

    private GridMover player;
    public float sightDistance = 10;
    public int peripheralVision = 30;

    private bool targetSighted = false;
    private Vector2 currentDirection;
    public Rotator rotator;
    private AutoMover mover;
    private LayerMask mask; //~(1 << 9) & ~(1 << 10) & ~(1 << 11);

    // Start is called before the first frame update
    void Start()
    {
        //subjects = new List<GameObject>();
        //subjects.Add(PlayerMover.instance.gameObject);

        mask = LayerMask.GetMask(new string[] { "Default", "Player" });
        player = PlayerMover.instance.GetComponent<GridMover>();
        mover = GetComponent<AutoMover>();
    }

    // Update is called once per frame
    void Update()
    {

        // This is going to get really hard. Check for player AND dead bodies
        // Behave differently when a body is spotted.

        //for(int i = 0; i < subjects.Count; ++i)
        //{
            Vector2 origin = transform.position;// + rotator.FrontOffset();
            Vector2 direction = (Vector2)player.transform.position - origin;

            targetSighted = false;

            float angle = Vector2.Angle(rotator.FrontOffset(), direction);

            // If player is in cone of vision, and there is an uninterrupted view
            // Or, if player is touching
            if ((angle < peripheralVision && ClearShot(origin, direction))
            || GridMover.Touching(gameObject, player.gameObject))
            {
                //rotator.FacePoint(player.GetDiscretePosition());
                targetSighted = true;
                location = player.GetDiscretePosition();
                // AutoMover should determine what to do with this information
                //mover.SetChasing(true);
            }

            /*if(Vector2.Distance(transform.position, player.transform.position) < 1.5f)
            {
                targetSighted = true;
                location = player.GetDiscretePosition();
                rotator.FacePoint(player.GetDiscretePosition());
                mover.SetChasing(true);
            }*/
        //}
    }

    public bool ClearShot(Vector2 origin, Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, sightDistance, mask);
        return hit.collider != null && hit.collider.CompareTag("Player");
    }

    public bool GetSighted()
    {
        return targetSighted;
    }

    public Vector2 SubjectLocation()
    {
        if (targetSighted)
        {
            return location;
        }
        Debug.Log("ERROR: Returned location of unseen subject.");
        return Vector2.zero;
    }
}
