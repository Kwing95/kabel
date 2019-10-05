using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigator : MonoBehaviour
{
    public const int INFINITY = 1073741823;
    public const int QUEUE_MAX = 45;
    public readonly Vector2 NULL_VECTOR = new Vector2(-INFINITY, -INFINITY);
    public GameObject noise;

    private GridMover mover;

    private Vector2 destination;
    private bool moving = false;
    private bool running = false;
    private FieldUnit unit;

    // public GameObject marker;

    // Start is called before the first frame update
    void Start()
    {
        unit = GetComponent<FieldUnit>();
        mover = GetComponent<GridMover>();
        destination = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float distToDest = Vector2.Distance(transform.position, destination);

        if (mover.GetCanTurn())
        {
            List<int> path = Grapher.FindPath(Grapher.VectorToIndex((Vector2)transform.position), Grapher.VectorToIndex(destination));

            if (path.Count > 1)
            {
                moving = true;
                mover.ChangeDirection(Grapher.graph[path[1]] - (Vector2)transform.position);
                if (running)
                {
                    GameObject tempNoise = Instantiate(noise, transform.position, Quaternion.identity);
                    tempNoise.GetComponent<Noise>().Initialize(CompareTag("Player"), 3);
                }
            } else if(moving)
            {
                if (tag == "Player")
                {
                    if (PlayerMover.instance.GetComponent<FieldUnit>().ap == 0)
                        CommandManager.EndTurn();
                    else
                        MenuNode.RefreshMenu();
                }
                moving = false;
            }
        }

        //Debug.Log(transform.position + " to " + destination + " = " + distToDest.ToString() + " moving: " + moving.ToString());
    }

    public void SetDestination(Vector2 dest, bool run = false)
    {
        running = run;
        // Trying to use more AP than you have ends your turn
        if (!this.CompareTag("Player") && unit.ap == 1 && !run)
        {
            unit.EndTurn();
            return;
        }
        destination = dest;
    }

}