using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMover : MonoBehaviour
{
    public const int INFINITY = 1073741823;
    public const int QUEUE_MAX = 45;
    public readonly Vector2 NULL_VECTOR = new Vector2(-INFINITY, -INFINITY);
    public static readonly List<Vector2> cardinals = 
        new List<Vector2>(new Vector2[] { Vector2.right, Vector2.left,
            Vector2.up, Vector2.down });
    public bool verbose = false;

    public List<Vector2> route;
    public AudioClip alarm;
    public AudioClip punch;
    public GameObject cross;
    public Rotator rotator;
    public SightVisualizer visualizer;
    public Navigator navigator;
    private FieldUnit unit;

    private GridMover mover;
    private GridMover player;
    private GameObject crossInstance;

    private Seeker seeker;

    private AudioSource source;
    private int startAngle;
    private int routeProgress = 0;
    private List<Vector2> graph;
    private List<int> dist = new List<int>();
    private List<int> prev = new List<int>();
    private Vector2 destination;
    private bool chasing = false;
    private Shooter shooter;

    private Vector2 savedDestination;

   // public GameObject marker;

    // Start is called before the first frame update
    void Start()
    {
        crossInstance = Instantiate(cross, transform.position, Quaternion.identity);
        crossInstance.GetComponent<SpriteRenderer>().enabled = false;

        unit = GetComponent<FieldUnit>();
        navigator = GetComponent<Navigator>();
        mover = GetComponent<GridMover>();
        source = GetComponent<AudioSource>();
        shooter = GetComponent<Shooter>();
        seeker = GetComponent<Seeker>();
        player = PlayerMover.instance.GetComponent<GridMover>();
        startAngle = rotator.GetAngle();
        if (route.Count == 0)
            route.Add(transform.position);

        //mover = GetComponent<GridMover>();
        destination = transform.position;
        SetChasing(false);
    }

    // Update is called once per frame
    void Update()
    {

        float distanceToPlayer = Vector2.Distance(transform.position, player.GetDiscretePosition());

        if(TouchingPlayer())
        {
            SetChasing(true);
            SetDestination(player.transform.position, true);
        }

        // This should run once every time the enemy moves
        if (unit.GetAP() > 0 && mover.GetCanTurn() && (distanceToPlayer > 1 || !chasing))
        {
            EnemyTurn();
        }
    }

    private void EnemyTurn()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.GetDiscretePosition());

        if (seeker.GetSighted() || chasing)
        {
            //destination = seeker.SubjectLocation();
            chasing = true;
        }
        //visualizer.SetAlert(chasing);

        if (unit.GetAP() > 2 && seeker.GetSighted())
        {
            EnemyAttack();
        }
        if(unit.GetAP() > 0 && (distanceToPlayer > 1 || !chasing))
        {
            EnemyMove();
        }
        if (unit.GetAP() <= 0)
        {
            EnemyList.SetEnemiesMoving(false);
        }
    }

    private void EnemyMove()
    {

        if (!chasing)
        {
            destination = route[routeProgress % route.Count];
            if (Vector2.Distance(transform.position, destination) == 0)
            {
                ++routeProgress;
                if (route.Count == 1 && !chasing)
                    rotator.Rotate(startAngle);
            }
        }

        unit.ConsumeAP(chasing ? 1 : 2);
        List<int> path = Grapher.FindPath(Grapher.VectorToIndex((Vector2)transform.position), Grapher.VectorToIndex(destination));
        // If unit has somewhere it wants to go
        if (path.Count > 1)
        {
            int nextTileIndex = path[1];
            Vector2 nextTile = Grapher.GetGraph()[nextTileIndex];
            navigator.SetDestination(nextTile, chasing);
        }
        else
        {
            if (chasing)
            {
                crossInstance.GetComponent<SpriteRenderer>().enabled = false;
            }
            SetChasing(false);
            if (route.Count == 1)
            {
                unit.EndTurn();
            }
            destination = route[routeProgress % route.Count];
        }
    }

    private void EnemyAttack()
    {
        if(unit.GetAP() >= 2)
        {
            unit.ConsumeAP(2);
            shooter.FireShot();
        }
    }

    private bool TouchingPlayer()
    {
        return Mathf.Abs(transform.position.x - player.transform.position.x) <= 1 &&
            Mathf.Abs(transform.position.y - player.transform.position.y) <= 1;
    }

    public void SetChasing(bool value)
    {
        if (value)
        {
            if (!source.isPlaying)
            {
                source.PlayOneShot(alarm);
                source.Play();
            }
        }

        chasing = value;
        visualizer.SetAlert(value);
    }

    public void SetDestination(Vector2 dest, bool detected=false)
    {
        destination = dest;
        if (detected)
        {
            crossInstance.transform.position = dest;
            crossInstance.GetComponent<SpriteRenderer>().enabled = true;
        }
    }

}
