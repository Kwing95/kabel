using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideMover : MonoBehaviour
{

    public int maximumFleeDistance;
    private Navigator navigator;
    private GridMover mover;
    private Rotator rotator;
    private FieldOfView fieldOfView;
    private float glanceTimer = 0;
    private static float glanceLength = 1;
    public Vector2 lastSawPlayer;
    private float sightDistance = 10;
    private bool hasFiredFlare = false;
    private float flareDelay = 5;
    private float timeAlone = 0;
    private bool lookTowardPlayer = true;

    // Start is called before the first frame update
    void Awake()
    {
        navigator = GetComponent<Navigator>();
        fieldOfView = GetComponentInChildren<FieldOfView>();
        rotator = GetComponent<Rotator>();
        mover = GetComponent<GridMover>();
    }

    void Start()
    {
        if (ClearView(transform.position, PlayerMover.instance.transform.position))
        {
            lastSawPlayer = Grapher.RoundedVector(PlayerMover.instance.transform.position);
            navigator.Hide(PlayerMover.instance.transform.position);
        }
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

    // Update is called once per frame
    void Update()
    {
        // If it sees player, run from player
        if (CanSeePoint(PlayerMover.instance.transform.position))
        {
            lastSawPlayer = Grapher.RoundedVector(PlayerMover.instance.transform.position);
            navigator.Hide(PlayerMover.instance.transform.position);
            timeAlone = 0;
        }
        else if (mover.GetCanTurn())
        {
            timeAlone += Time.deltaTime;

            if(timeAlone > flareDelay && !hasFiredFlare)
                Flare();

            if (glanceTimer > 0)
                glanceTimer -= Time.deltaTime;
            else
            {
                if(lookTowardPlayer)
                    rotator.FacePoint(lastSawPlayer);
                else
                    rotator.Rotate(Random.Range(0, 360));

                lookTowardPlayer = !lookTowardPlayer;
                glanceTimer = glanceLength;
            }
        }
    }

    private void Flare()
    {
        Vector2 roundedPosition = Grapher.RoundedVector(transform.position);
        GameObject tempNoise = Instantiate(Globals.NOISE, roundedPosition, Quaternion.identity);
        tempNoise.GetComponent<Noise>().Initialize(true, Globals.FLARE_VOLUME, Noise.Source.Gun);
        
        SpawnService.instance.SpawnEnemy(roundedPosition);
        hasFiredFlare = true;
    }

    private bool CanSeePoint(Vector2 point)
    {
        // Check that player is within view angle
        Vector2 direction = point - (Vector2)transform.position;
        float angleToPoint = Mathf.Abs(Vector2.SignedAngle(direction, rotator.GetCurrentAngleVector()));
        
        return angleToPoint < fieldOfView.viewAngle / 2 && ClearView(transform.position, point) && Vector2.Distance(point, transform.position) <= sightDistance;
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
