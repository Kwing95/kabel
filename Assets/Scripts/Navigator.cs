using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class Navigator : MonoBehaviour
{
    public int maxPathLength = -1;
    public bool usesMultithreading = true;
    public bool waypointEnabled = false;

    public const int INFINITY = 1073741823;
    public const int QUEUE_MAX = 45;
    public readonly Vector2 NULL_VECTOR = new Vector2(-INFINITY, -INFINITY);

    private GridMover mover;

    // destination is ONLY used for generating a new path (when destinationQueued is true)
    // AutoMover also uses it to see if destination is unobstructed
    private Vector2 destination;
    private bool running = false;
    private bool pausePathFinding = false;
    private FieldUnit unit;
    private GameObject waypoint;

    List<Vector2> path;
    private int pathProgress = 1;
    private bool destinationQueued = false;
    private bool idle = true;

    private int nonce = 0;

    // public GameObject marker;

    // Start is called before the first frame update
    void Start()
    {
        unit = GetComponent<FieldUnit>();
        mover = GetComponent<GridMover>();
        destination = transform.position;
        path = new List<Vector2>();
    }

    // Update is called once per frame
    void Update()
    {
        // float distToDest = Vector2.Distance(transform.position, destination);

        if (mover.GetCanTurn() && !pausePathFinding)
        {
            // If destination has been set recently, generate a new path
            // destinationQueued is true when "destination" is outdated and a new path must be generated
            if(destinationQueued)
            {
                path = Grapher.FindPath(transform.position, destination, maxPathLength);

                if(path.Count > 0 && waypointEnabled)
                {
                    Destroy(waypoint);
                    waypoint = Instantiate(Globals.WAYPOINT, destination, Quaternion.identity);
                }
                    
                pathProgress = 1;
                destinationQueued = false;
            }

            // If path does not exist, stop trying to navigate there
            if (path.Count == 0)
            {
                if(waypointEnabled)
                    Destroy(waypoint);
                pausePathFinding = true;
            }
                
            // Move along path
            if (path.Count > 1){
                if (pathProgress < path.Count)
                {
                    //mover.
                    mover.ChangeDirection(path[pathProgress] - (Vector2)transform.position, running);
                    pathProgress += 1;

                    if (running)
                    {
                        GameObject tempNoise = Instantiate(Globals.NOISE, transform.position, Quaternion.identity);
                        tempNoise.GetComponent<Noise>().Initialize(CompareTag("Player"), 3.5f); // bad
                    }
                }
                else
                {
                    // Upon losing sight of player or reaching waypoint
                    idle = true;
                }
            }
        }
    }

    public void SetDestination(Vector2 dest, bool run=false)
    {
        if (dest == destination)
            return;

        // TODO testing
        /*if (GetComponent<AutoMover>() != null)
        {
            GetComponent<AutoMover>().crossInstance.transform.position = dest;
            GetComponent<AutoMover>().crossInstance.GetComponent<SpriteRenderer>().enabled = true;
        }*/

        if (usesMultithreading)
            StartCoroutine(SetDestinationMulti(dest, run));
        else
            SetDestinationSingle(dest, run);
    }

    // Single-threaded implementation
    public void SetDestinationSingle(Vector2 dest, bool run = false)
    {
        running = run; // check
        destination = dest;
        destinationQueued = true; // only needed while single-threading
        idle = false; // check

        pausePathFinding = false; // check
    }

    // Multi-threaded implementation
    public IEnumerator SetDestinationMulti(Vector2 dest, bool run = false)
    {
        bool done = false;

        // Halt the enemy while they're thinking
        path = new List<Vector2>();
        destinationQueued = false;

        nonce += 1;
        int lastKnownNonce = nonce;

        List<Vector2> newPath = new List<Vector2>();
        Vector2 start = mover.GetDiscretePosition();

        new Thread(() => {
            newPath = Grapher.FindPath(start, dest, maxPathLength);
            done = true;
        }).Start();

        while (!done)
            yield return null;

        // Debug.Log("Generated path of length " + newPath.Count + " from " + newPath[0] + " to " + newPath[newPath.Count - 1]);

        if (nonce == lastKnownNonce || true)
        {
            destination = dest;
            path = newPath;
            pathProgress = 1;
            running = run;
            idle = false;
            pausePathFinding = false;
        }
    }

    // Used to determine what AutoMover does when enemy is idle
    public bool GetIdle()
    {
        return idle;
    }

    public Vector2 GetDestination()
    {
        return destination;
    }

    public void PrintPath()
    {
        foreach (Vector2 elt in path)
            Debug.Log(elt);
    }

}