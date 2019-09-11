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
    private LayerMask mask = ~(1 << 9) & ~(1 << 10) & ~(1 << 11);

    // Start is called before the first frame update
    void Start()
    {
        //subjects = new List<GameObject>();
        //subjects.Add(PlayerMover.instance.gameObject);
        player = PlayerMover.instance.GetComponent<GridMover>();
        mover = GetComponent<AutoMover>();
    }

    // Update is called once per frame
    void Update()
    {
        //for(int i = 0; i < subjects.Count; ++i)
        //{
            Vector2 origin = transform.position;// + rotator.FrontOffset();
            Vector2 direction = (Vector2)player.transform.position - origin;

            targetSighted = false;

            float angle = Vector2.Angle(rotator.FrontOffset(), direction);
            if (angle < peripheralVision)
            {
                RaycastHit2D hit = Physics2D.Raycast(origin, direction, sightDistance, mask);
                if (hit.collider != null && hit.collider.CompareTag("Player"))
                {
                    targetSighted = true;
                    location = player.GetDiscretePosition();
                    // AutoMover should determine what to do with this information
                    mover.SetDestination(player.GetDiscretePosition(), true);
                    mover.SetChasing(true);
                }
            }
        //}
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.parent.transform.position, player.transform.position);
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                Debug.Log("Player detected!");
            }
        }
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
