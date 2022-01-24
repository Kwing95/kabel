using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;

public struct PathfindingJob : IJob
{
    public NativeList<Vector2> path;
    public Vector2 start;
    public Vector2 dest;
    public int maxPathLength;
    public int jobId;
    public bool running;
    public bool hiding;

    public void Execute()
    {
        if (hiding)
            dest = Grapher.HidingPlace(start, dest, maxPathLength);

        //Debug.Log("beginning pathfinding job from " + start + " to " + dest);

        List<Vector2> rawPath = Grapher.FindPath(start, dest, maxPathLength);
        if(rawPath.Count == 0)
        {
            Debug.Log("Failed to generate path from " + start + " to " + dest);
        }
        foreach (Vector2 elt in rawPath)
            path.Add(elt);
    }
}

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
    [SerializeField]
    private bool pausePathFinding = false;
    private GameObject waypoint;

    List<KeyValuePair<JobHandle, PathfindingJob>> activeJobs;
    [SerializeField]
    private List<Vector2> path;
    private int pathProgress = 1; // Index along path currently being navigated to
    [SerializeField]
    private bool destinationQueued = false; // Happens when destination has been changed but path has not been generated yet
    [SerializeField]
    private bool idle = true; // Set to true after Navigator finishes moving along path

    private int nonce = 0;

    // public GameObject marker;

    // Start is called before the first frame update
    void Start()
    {
        activeJobs = new List<KeyValuePair<JobHandle, PathfindingJob>>();
        mover = GetComponent<GridMover>();
        destination = transform.position;
        path = new List<Vector2>();
    }

    public void OnDestroy()
    {
        if (activeJobs != null)
        {
            foreach (KeyValuePair<JobHandle, PathfindingJob> job in activeJobs)
            {
                // WARNING: Game could slow down if enemy is destroyed during pathfinding operation
                job.Key.Complete();
                job.Value.path.Dispose();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckJobs();
        // If destination has been set recently, generate a new path
        // destinationQueued is true when "destination" is outdated and a new path must be generated
        if (destinationQueued)
        {
            Vector2 position = mover.GetDiscretePosition();
            path = Grapher.FindPath(position, destination, maxPathLength);

            if (path.Count > 0 && waypointEnabled)
            {
                Destroy(waypoint);
                waypoint = Instantiate(Globals.WAYPOINT, destination, Quaternion.identity);
            }

            if(path.Count > 1)
                pathProgress = Grapher.ManhattanDistance(path[0], transform.position) == 1 ? 0 : 1;
            destinationQueued = false;

            // If path does not exist, stop trying to navigate there
            if (path.Count == 0)
            {
                if (waypointEnabled)
                    Destroy(waypoint);
                Pause(true);
            }
        }

        if (mover.GetCanTurn() && !pausePathFinding && ActionManager.GetState() == ActionManager.State.Moving)
        {                
            // Move along path
            if (path.Count > 1){
                if (pathProgress < path.Count)
                {
                    Vector2 direction = path[pathProgress] - (Vector2)transform.position;

                    //if(pathProgress > 0 && (Vector2)transform.position != path[pathProgress - 1])
                    //    Debug.Log("WARNING: Expected position " + transform.position + " to be " + path[pathProgress - 1]);
                    mover.ChangeDirection(direction, running);
                    pathProgress += 1;

                    GameObject tempNoise = Instantiate(Globals.NOISE, transform.position, Quaternion.identity);
                    tempNoise.GetComponent<Noise>().Initialize(CompareTag("Player"), running ? Globals.RUN_VOLUME : Globals.WALK_VOLUME, Noise.Source.Footsteps); // bad
                }
                else
                {
                    // Upon losing sight of player or reaching waypoint
                    SetIdle(true);
                }
            }
        }
    }

    public void SetDestination(Vector2 dest, bool run=false)
    {
        dest = Grapher.RoundedVector(dest);
        if (dest == destination || (activeJobs != null && activeJobs.Count > 0))
            return;

        // TODO testing
        /*if (GetComponent<AutoMover>() != null)
        {
            GetComponent<AutoMover>().crossInstance.transform.position = dest;
            GetComponent<AutoMover>().crossInstance.GetComponent<SpriteRenderer>().enabled = true;
        }*/
        // Debug.Log(gameObject + " (Navigator): SetDestination from " + transform.position + " to " + dest);

        if (usesMultithreading)
            SetDestinationJob(dest, run);
            //StartCoroutine(SetDestinationMulti(dest, run));
        else
            SetDestinationSingle(dest, run);
    }

    // Single-threaded implementation
    public void SetDestinationSingle(Vector2 dest, bool run = false)
    {
        running = run; // check
        destination = dest;
        destinationQueued = true; // only needed while single-threading
        SetIdle(false); // check

        Pause(false); // check

    }

    public void Hide(Vector2 threat)
    {
        if (!mover.GetCanTurn())
            return;

        Vector2 destination = Grapher.HidingPlace(transform.position, threat, 10);
        if(destination != -1 * Vector2.one)
            SetDestination(destination);
        else
        {
            // Determine base direction of retreat
            bool axisIsX = Mathf.Abs(threat.x - transform.position.x) > Mathf.Abs(threat.y - transform.position.y);
            Vector2 offset;
            if (axisIsX)
                offset = threat.x < transform.position.x ? Vector2.right : Vector2.left;
            else
                offset = threat.y < transform.position.y ? Vector2.up : Vector2.down;

            // Project destination outward until a wall is hit
            int multiplier = 1;
            while(Grapher.PointIsClear((Vector2)transform.position + (offset * multiplier)))
            {
                destination = (Vector2)transform.position + (offset * multiplier);
                multiplier += 1;
            }
            if (Grapher.PointIsClear(destination))
                SetDestination(destination);
            else
                Pause(true); // Stuck

        }
        //SetDestinationJob(threat, false, true);
    }

    public void CheckJobs()
    {
        for (int i = 0; i < activeJobs.Count;)
        {
            if (activeJobs[i].Key.IsCompleted)
            {
                activeJobs[i].Key.Complete();

                //Debug.Log(name + "'s job finished with nonce = " + nonce + " and jobId = " + activeJobs[i].Value.jobId);
                if (nonce == activeJobs[i].Value.jobId/* || true*/)
                {
                    destination = activeJobs[i].Value.dest;

                    List<Vector2> convertedList = new List<Vector2>();
                    foreach (Vector2 elt in activeJobs[i].Value.path)
                        convertedList.Add(elt);
                    path = convertedList;

                    if (path.Count > 1)
                        pathProgress = Grapher.ManhattanDistance(path[0], transform.position) == 1 ? 0 : 1;
                    running = activeJobs[i].Value.running;
                    SetIdle(false);
                    Pause(false);
                }

                activeJobs[i].Value.path.Dispose();
                activeJobs.Remove(activeJobs[i]);
            }
            else
            {
                ++i;
            }
        }
    }

    public void SetDestinationJob(Vector2 dest, bool run=false, bool hide=false)
    {
        nonce += 1;
        int lastKnownNonce = nonce;

        NativeList<Vector2> rawList = new NativeList<Vector2>(Allocator.TempJob);

        //Debug.Log(name + "'s job started with jobId = " + lastKnownNonce);

        // Set up the job data
        PathfindingJob fetcher = new PathfindingJob
        {
            start = mover.GetDiscretePosition(),
            dest = dest,
            maxPathLength = maxPathLength,
            path = rawList,
            jobId = lastKnownNonce,
            running = run,
            hiding = hide
        };

        // Schedule the job
        JobHandle handle = fetcher.Schedule();

        activeJobs.Add(new KeyValuePair<JobHandle, PathfindingJob>(handle, fetcher));

        //if (!usesMultithreading)
        //    handle.Complete();
    }



    // Multi-threaded implementation
    public IEnumerator SetDestinationMulti(Vector2 dest, bool run = false)
    {
        Debug.Log("SetDestinationMulti called");
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
            yield return new WaitForEndOfFrame();

        // Debug.Log("Generated path of length " + newPath.Count + " from " + newPath[0] + " to " + newPath[newPath.Count - 1]);

        if (nonce == lastKnownNonce || true)
        {
            destination = dest;
            path = newPath;
            if (path.Count > 1)
                pathProgress = Grapher.ManhattanDistance(path[0], transform.position) == 1 ? 0 : 1;
            running = run;
            SetIdle(false);
            Pause(false);
        }
    }

    public void Pause(bool setPaused=true)
    {
        pausePathFinding = setPaused;
    }

    public bool GetPaused()
    {
        return pausePathFinding;
    }

    public int GetPathLength()
    {
        return path.Count;
    }

    public void SetIdle(bool value)
    {
        idle = value;
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