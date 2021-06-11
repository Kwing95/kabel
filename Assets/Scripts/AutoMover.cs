using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class AutoMover : MonoBehaviour
{
    public const int INFINITY = 1073741823;

    public List<Vector2> route;

    private Rotator rotator;

    public int leashLength = -1;
    public bool randomPatrol = false;
    public int pointMemory = 0;
    public float sightDistance = 10;
    public float attackRate = 1;

    private bool clearView;
    private bool canSeePlayer;
    private bool stoppedByLeash = false;
    private float attackCooldown = 0;
    private float stunTimer = 0;
    private List<Vector2> extraPoints;
    private bool canAddPoints = true;

    private GridMover player;
    private LayerMask mask;
    private GridMover mover;
    private Navigator navigator;
    private FieldUnit unit;

    private AudioSource source;
    public int startAngle = 0;
    private int routeProgress = 0;
    private FieldOfView fieldOfView;

    public enum State { Stun, Idle, Confuse, Suspicious, Alert } // stun 0, alert 4
    [SerializeField]
    private State awareness;
    // PRIORITY: Stun > Alert > Suspicious > Idle
    // Idle -> Suspicious, Alert, Stun              Standing, patroling, or returning to patrol
    // Suspicious -> Alert, Confuse, Stun           Investigating noise, sighting, or corpse
    // Alert -> Suspicious, Confuse, Stun           Chasing and attacking player
    // Confuse -> Idle, Suspicious, Alert, Stun     Looking around after reaching origin of suspicious activity
    // Stun -> Suspicious                           Stunned after stun grenade
    // FieldOfView is only modified through SetMaterial; no state stuff needed

    // leashLength          How far enemy will drift from its patrol; -1 for unlimited
    // addsPoints           When true, enemy will add location of dead body to patrol
    // randomPatrol         When true, enemy will cycle randomly through waypoints
    // tense                When true, enemy will always run after first sighting

   // public GameObject marker;

    // Start is called before the first frame update
    void Awake()
    {
        mask = LayerMask.GetMask(new string[] { "Default", "Player" });
        
        extraPoints = new List<Vector2>();
        fieldOfView = GetComponentInChildren<FieldOfView>();
        rotator = GetComponent<Rotator>();
        unit = GetComponent<FieldUnit>();
        mover = GetComponent<GridMover>();
        navigator = GetComponent<Navigator>();
        source = GetComponent<AudioSource>();
        player = PlayerMover.instance.GetComponent<GridMover>();

        transform.rotation = Quaternion.Euler(Vector3.zero);

        if (route.Count == 0)
            route.Add(transform.position);        
    }

    void Start()
    {
        SetAwareness(State.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        clearView = ClearView();
        canSeePlayer = CanSeePlayer();

        if (stunTimer <= 0)
        {
            if (awareness == State.Stun)
                SetAwareness(State.Suspicious);
            attackCooldown -= Time.deltaTime;
        }
        else
        {
            stunTimer -= Time.deltaTime;
            // Only execute right when stunTimer flips to 0
            if (stunTimer <= 0)
                navigator.Pause(false);
        }
            
        //fieldOfView.material

        CheckForTarget();

        // If the player can be seen attack them
        // THIS DOES NOT ACCOUNT FOR ANGLE
        if (attackCooldown <= 0 && canSeePlayer)
        {
            AlertToPosition(player.GetDiscretePosition());
            Attack();
        }

        // Resolve direction enemy should be facing (lock onto player while alerted)
        Vector2 destination = navigator.GetDestination();

        // Lock on if suspicious or alert, AND clear view to point of interest,
        // Lock on if player can be seen, OR distance away is > 1
        Vector2 lockOnPoint = canSeePlayer ? (Vector2)player.transform.position : destination;
        bool closeLockOn = Grapher.ManhattanDistance(destination, transform.position) > 1 || canSeePlayer;
        bool lockedOn = awareness >= State.Suspicious && ClearView(destination) && closeLockOn;
        //Debug.Log(lockedOn);
        rotator.ToggleLock(awareness >= State.Suspicious && ClearView(destination) && closeLockOn, lockOnPoint);

        /* The two above should probably be combined, but we want to address the case where an enemy might be in the way of another enemy...
         Double check if mask makes this possible? Enemies should be able to hit each other, but NOT if they're overlapping each other. 
         Could fake raycast to start one normalized length away from center point? */

        // Move toward position (maybe create EnemyNavigator derived from Navigator, replaces AutoMover)
        float distanceToPlayer = Vector2.Distance(transform.position, player.GetDiscretePosition());

        EnemyMove();
    }

    private void OnDisable()
    {
        mover.enabled = false;
        rotator.enabled = false;
        fieldOfView.enabled = false;
    }

    private void OnEnable()
    {
        mover.enabled = true;
        rotator.enabled = true;
        fieldOfView.enabled = true;
    }

    // Primarily affects movement when behavior needs to change
    private void EnemyMove()
    {
        if (DistanceFromPlayer() <= 1.5f)
        {
            // AlertToPosition(player.transform.position);
            rotator.FacePoint(player.transform.position);
            SetAwareness(State.Alert);
        }

        if(awareness == State.Alert &&
            route.Count == 1 &&
            leashLength != -1 &&
            Grapher.ManhattanDistance(route[0], transform.position) >= leashLength
            /*Grapher.FindIndirectPath(route[0], transform.position, leashLength).Count == 0*/)
        {
            stoppedByLeash = true;
            navigator.Pause();
        }
        if (awareness == State.Suspicious && (navigator.GetIdle() || (stoppedByLeash && !canSeePlayer) || navigator.GetPaused())) // should fix hang
        {
            stoppedByLeash = false;
            SetAwareness(State.Confuse);
            StartCoroutine(LookAround());
        }
        else if (awareness == State.Idle && navigator.GetIdle())
        {
            // If the enemy is not chasing the player, patrol duty
            Vector2 destination = GetDestination();
            navigator.SetDestination(destination, false);
            if (Vector2.Distance(transform.position, destination) == 0)
            {
                routeProgress = randomPatrol ? Random.Range(0, route.Count + extraPoints.Count) : (routeProgress + 1) % route.Count;

                // If returning to stationary post
                if (route.Count == 1 && awareness <= State.Idle)
                    rotator.Rotate(startAngle);
            }
        }
    }

    // Probably needs to have its own boolean so other lock-on doesn't overwrite this
    public IEnumerator Glance(Vector2 point, float delay=2f)
    {
        rotator.ToggleLock(true, point);
        yield return new WaitForSeconds(delay);
        rotator.ToggleLock(false, point);
    }

    public IEnumerator LookAround(int numDirections=3, float delay=1f)
    {
        if (leashLength == -1)
        {
            yield return new WaitForSeconds(delay);
            for (int i = 0; i < numDirections; ++i)
            {
                // If awareness changes during coroutine, exit early
                if (awareness > State.Confuse)
                    yield break;

                rotator.Rotate(Random.Range(0, 360));
                yield return new WaitForSeconds(delay);
            }
        }
        navigator.SetDestination(GetDestination(), false); // comment out?
        canAddPoints = true;
        SetAwareness(State.Idle);
        navigator.SetIdle(true);
        navigator.Pause(false);
    }
    
    private Vector2 GetDestination()
    {
        return routeProgress < route.Count ? route[routeProgress] : extraPoints[routeProgress - route.Count];
    }

    private void CheckForTarget()
    {
        if (canSeePlayer)
        {
            //rotator.ToggleLock(true, player.transform.position);
            AlertToPosition(player.GetDiscretePosition());
        }
        else if (awareness == State.Alert)
            SetAwareness(State.Suspicious);
    }

    // Returns true if player is within enemy's cone of vision
    private bool CanSeePlayer()
    {
        // Check that player is within view angle
        Vector2 direction = player.transform.position - transform.position;
        float angleToPlayer = Mathf.Abs(Vector2.SignedAngle(direction, rotator.GetCurrentAngleVector()));

        return angleToPlayer < fieldOfView.viewAngle / 2 && clearView && Vector2.Distance(player.transform.position, transform.position) <= sightDistance;
    }

    // Returns true if there are no obstructions between enemy and player
    private bool ClearView()
    {
        return ClearView(player.transform.position);
    }

    private bool ClearView(Vector2 point)
    {
        // Don't look farther than distance of point of interest, but don't look farther than sightDistance either
        float effectiveDistance = Mathf.Min(Vector2.Distance(transform.position, point), sightDistance);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, point - (Vector2)transform.position, effectiveDistance, mask);

        bool returnValue = hit.collider == null || hit.collider.CompareTag("Player");
        return returnValue;
    }

    // Makes enemy investigate "point"
    public void AlertToPosition(Vector2 point)
    {
        bool tooFar = false;
        if(route.Count == 1 && leashLength != -1)
        {
            tooFar = Grapher.ManhattanDistance(route[0], canSeePlayer ? (Vector2)transform.position : point) >= leashLength;
            //tooFar = Grapher.FindIndirectPath(route[0], canSeePlayer ? (Vector2)transform.position : point, leashLength).Count == 0;
        }

        // Ignore noises outside of patrol zone... Should they at least look?
        if(!canSeePlayer && tooFar)
        {
            if (clearView)
            {
                SetAwareness(State.Suspicious);
                navigator.SetDestination(route[0], true);
            }
            // Possibly call in backup
            return;
        }
        if (canSeePlayer)
        {
            if (canAddPoints && pointMemory > 0)
            {
                canAddPoints = false;
                extraPoints.Add(Grapher.RoundedVector(player.transform.position));
                if (extraPoints.Count > pointMemory)
                    extraPoints.RemoveAt(0);
            }
            SetAwareness(State.Alert);
            // Don't chase player when close

            bool tooClose = ClearView(player.GetDiscretePosition()) && Grapher.ManhattanDistance(player.transform.position, transform.position) <= 4;
            if (tooClose || tooFar)
            {
                navigator.Pause();
                return;
            }
        }
        else
        {
            SetAwareness(State.Suspicious);
        }

        navigator.SetDestination(point, true);
    }

    private void Attack()
    {
        if (DistanceFromPlayer() <= 1)
            rotator.FacePoint(player.transform.position);

        ActionManager.Gun(gameObject, player.transform.position);
        attackCooldown = attackRate;
    }

    // Return floating-point distance from player
    private float DistanceFromPlayer()
    {
        return Vector2.Distance(player.transform.position, transform.position);
    }

    private void Stun(float duration)
    {
        SetAwareness(State.Stun);
        stunTimer = Mathf.Max(stunTimer, duration);
        navigator.Pause();
    }

    private void SetAwareness(State state)
    {
        awareness = state;

        switch (state)
        {
            case State.Idle:
                fieldOfView.SetAlert(0);
                break;
            case State.Suspicious:
                fieldOfView.SetAlert(1);
                break;
            case State.Alert:
                fieldOfView.SetAlert(2);
                break;
        }
    }

    public State GetAwareness()
    {
        return awareness;
    }

}
