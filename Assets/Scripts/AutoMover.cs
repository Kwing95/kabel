using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class AutoMover : MonoBehaviour
{
    public const int INFINITY = 1073741823;

    public List<Vector2> route;
    public AudioClip alarm;
    public AudioClip punch;

    private Rotator rotator;

    public float sightDistance = 10;
    public float attackRate = 1;
    private float attackCooldown = 0;

    private GridMover player;
    private GameObject crossInstance;
    private LayerMask mask;
    private GridMover mover;
    private Navigator navigator;
    private FieldUnit unit;

    private AudioSource source;
    private int startAngle;
    private int routeProgress = 0;
    private bool chasing = false;
    public bool waiting = false;
    private FieldOfView fieldOfView;

   // public GameObject marker;

    // Start is called before the first frame update
    void Awake()
    {
        //EnemyList.numEnemies += 1; // EnemyList counts this itself
        // crossInstance = Instantiate(Globals.WAYPOINT, transform.position, Quaternion.identity);
        mask = LayerMask.GetMask(new string[] { "Default", "Player" });

        fieldOfView = GetComponentInChildren<FieldOfView>();
        rotator = GetComponent<Rotator>();
        unit = GetComponent<FieldUnit>();
        mover = GetComponent<GridMover>();
        navigator = GetComponent<Navigator>();
        source = GetComponent<AudioSource>();
        player = PlayerMover.instance.GetComponent<GridMover>();

        startAngle = Mathf.RoundToInt(transform.rotation.eulerAngles.z); // rotator.GetDestinationAngle();
        transform.rotation = Quaternion.Euler(Vector3.zero);

        if (route.Count == 0)
            route.Add(transform.position);

        //mover = GetComponent<GridMover>();
        SetChasing(false);
    }

    // Update is called once per frame
    void Update()
    {
        attackCooldown -= Time.deltaTime;
        // This should run once every time the enemy moves
        // (distanceToPlayer > 1 || !chasing) needs to be changed at some point
        EnemyTurn();
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

    private void OnDestroy()
    {
        Destroy(crossInstance);
    }

    private void EnemyTurn()
    {
        /*if (seeker.GetSighted() || TouchingPlayer())
        {
            rotator.FacePoint(player.transform.position);
            SetChasing(true);
        }*/

        CheckForTarget();

        // Attack the player if applicable
        RaycastHit2D hit = Physics2D.Raycast(transform.position, player.transform.position - transform.position, sightDistance, mask);
        bool clearShot = hit.collider != null && hit.collider.CompareTag("Player");
        
        if (attackCooldown <= 0 && chasing && (TouchingPlayer() || clearShot))
        {
            navigator.SetDestination(player.GetDiscretePosition(), true);
            EnemyAttack();
        }

        // Resolve direction enemy should be facing (lock onto player while alerted)
        Vector2 destination = navigator.GetDestination();
        hit = Physics2D.Raycast(transform.position, destination - (Vector2)transform.position, sightDistance, mask);

        if (chasing && (hit.collider == null || hit.collider.CompareTag("Player")))
            rotator.EnableLock(destination);
        else
            rotator.DisableLock();

        // Move toward position (maybe create EnemyNavigator derived from Navigator, replaces AutoMover)
        float distanceToPlayer = Vector2.Distance(transform.position, player.GetDiscretePosition());
        if (distanceToPlayer > 1 || !chasing)
            EnemyMove();
            
    }

    private void EnemyMove()
    {
        if (TouchingPlayer())
        {
            rotator.FacePoint(player.transform.position);
            SetChasing(true);
        }

        //Debug.Log(navigator.path);

        if (chasing)
        {
            if (navigator.GetIdle())
            {
                navigator.SetDestination(route[routeProgress], false);
                SetChasing(false);
            }
        }
        // If the enemy is not chasing the player, patrol duty
        else if (navigator.GetIdle())
        {
            Vector2 destination = route[routeProgress];
            navigator.SetDestination(destination, false);
            if (Vector2.Distance(transform.position, destination) == 0)
            {
                routeProgress = (routeProgress + 1) % route.Count;

                // If returning to stationary post
                if (route.Count == 1 && !chasing)
                    rotator.Rotate(startAngle);
            }
        }
    }

    private void CheckForTarget()
    {
        Vector2 direction = player.transform.position - transform.position;

        // Check for clear line of sight
        LayerMask mask = mask = LayerMask.GetMask(new string[] { "Default", "Player" });
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, sightDistance, mask);
        bool clearShot = hit.collider != null && hit.collider.CompareTag("Player");

        // If player is within view or touching
        float angleToPlayer = Vector2.Angle(Vector2.up, direction); // arg1 formerly rotator.FrontOffset()
        float enemyAngle = Rotator.mod((int)-rotator.GetCurrentAngle(), 360);

        // Debug.Log(enemyAngle + " - " + angleToPlayer + " = " + (enemyAngle - angleToPlayer));

        if (Mathf.Abs(enemyAngle - angleToPlayer) < fieldOfView.viewAngle / 2 && clearShot)
        {
            //Debug.Log("player spotted!");
            navigator.SetDestination(player.GetDiscretePosition(), true);
            SetChasing(true);
        }
            
    }

    public void StopWaiting()
    {
        waiting = false;
    }

    private void EnemyAttack()
    {
        // might need to add cooldown
        if (TouchingPlayer())
            rotator.FacePoint(player.transform.position);

        waiting = true;
        ActionManager.Gun(gameObject, player.transform.position);
        attackCooldown = attackRate;
    }

    private bool TouchingPlayer()
    {
        return Mathf.Abs(transform.position.x - player.transform.position.x) <= 1 &&
            Mathf.Abs(transform.position.y - player.transform.position.y) <= 1;
    }

    public void SetChasing(bool value)
    {
        /*if (value)
        {
            if (!source.isPlaying)
            {
                //source.PlayOneShot(alarm);
                source.Play();
            }
        }*/
        chasing = value;
        // visualizer.SetAlert(value);
    }

}
