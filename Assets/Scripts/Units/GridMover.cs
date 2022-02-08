using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* GridMover makes an object move until it hits a tile-based boundary, then halts movement.
 * 
 * GetDiscretePosition()
 *      Returns position of object. If object is moving, returns position object en route
 *      to occupy.
 * GetCanTurn()
 *      Returns true if object is stationary and can turn.
 * ChangeDirection(Vector2 direction)
 *      If object is stationary, object begins to move in the direction passed to function.
 *      Returns true if movement is produced, false if object is already busy moving.
     */

public class GridMover : MonoBehaviour
{

    public delegate void VoidVector2Param(Vector2 position);
    public VoidVector2Param onTileSnap;

    public float baseWalkSpeed = 4f;
    public float baseRunSpeed = 6f;

    private float walkSpeed;
    private float runSpeed;

    private bool blocksGraph = false;
    private bool canTurn = true;

    // Bounds are thresholds for how far GridMover must go before it's done with a move operation
    private char boundDirection;
    private float bound;

    private Rigidbody2D rb;
    private Vector2 prevDiscretePosition;
    private Vector2 nextDiscretePosition;
    private Vector2 heldVelocity;
    public Rotator rotator;

    // Start is called before the first frame update
    void Awake()
    {
        //rotator = GetComponent<Rotator>();
        rb = GetComponent<Rigidbody2D>();
        heldVelocity = Vector2.zero;

        ModifySpeed(0);
    }

    private void Start()
    {
        nextDiscretePosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!canTurn)
            CheckForFinishMove();
    }

    private void OnDisable()
    {
        // Debug.Log("disabling gridmover");
        heldVelocity = rb.velocity;
        rb.velocity = Vector2.zero;
    }

    private void OnEnable()
    {
        rb.velocity = heldVelocity;
    }

    private void CheckForFinishMove()
    {
        switch (boundDirection)
        {
            case 'U':
                if (transform.position.y >= bound)
                {
                    transform.position = new Vector2(transform.position.x, bound);
                    FinishMove();
                }
                break;
            case 'D':
                if (transform.position.y <= bound)
                {
                    transform.position = new Vector2(transform.position.x, bound);
                    FinishMove();
                }
                break;
            case 'R':
                if (transform.position.x >= bound)
                {
                    transform.position = new Vector2(bound, transform.position.y);
                    FinishMove();
                }
                break;
            case 'L':
                if (transform.position.x <= bound)
                {
                    transform.position = new Vector2(bound, transform.position.y);
                    FinishMove();
                }
                break;
        }
    }

    private void FinishMove()
    {
        //onTileSnap(transform.position);
        transform.position = new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));

        //Grapher.PrintGraph();
        if (blocksGraph)
        {
            Grapher.graph[(int)prevDiscretePosition.y, (int)prevDiscretePosition.x] = true;
            Grapher.graph[(int)nextDiscretePosition.y, (int)nextDiscretePosition.x] = false;
        }
        //Grapher.PrintGraph();

        canTurn = true;
        rb.velocity = Vector2.zero;
    }

    public static bool Touching(GameObject a, GameObject b)
    {
        return Mathf.Abs(a.transform.position.x - b.transform.position.x) <= 1 &&
            Mathf.Abs(a.transform.position.y - b.transform.position.y) <= 1;
    }

    // Move a single tile in "direction"
    // Returns true if movement was produced
    public bool ChangeDirection(Vector2 direction, bool running=false)
    {
        float moveSpeed = running ? runSpeed : walkSpeed;
        
        if (canTurn /*&& PointClear(direction)*/)
        {
            float volume = (running ? 1 : 0.5f) - (0.05f * Vector2.Distance(transform.position, PlayerMover.instance.transform.position));
            if(volume > 0)
                SoundManager.instance.Play(SoundManager.Sound.Step, -1, volume);

            if(direction == Vector2.up)
            {
                rb.velocity = Vector2.up * moveSpeed;
                nextDiscretePosition = (Vector2)transform.position + Vector2.up;
                bound = transform.position.y + 1;
                boundDirection = 'U';
                Rotate(0);
            }
            else if(direction == Vector2.down)
            {
                rb.velocity = Vector2.down * moveSpeed;
                nextDiscretePosition = (Vector2)transform.position + Vector2.down;
                bound = transform.position.y - 1;
                boundDirection = 'D';
                Rotate(180);
            }
            else if(direction == Vector2.right)
            {
                rb.velocity = Vector2.right * moveSpeed;
                nextDiscretePosition = (Vector2)transform.position + Vector2.right;
                bound = transform.position.x + 1;
                boundDirection = 'R';
                Rotate(270);
            }
            else if(direction == Vector2.left)
            {
                rb.velocity = Vector2.left * moveSpeed;
                nextDiscretePosition = (Vector2)transform.position + Vector2.left;
                bound = transform.position.x - 1;
                boundDirection = 'L';
                Rotate(90);
            }
            else
            {
                Debug.Log(gameObject.name + " at " + transform.position + " called ChangeDirection with direction " + direction);
                return false;
            }

            prevDiscretePosition = transform.position;
            if(blocksGraph)
                Grapher.graph[(int)transform.position.y, (int)transform.position.x] = false;

            // Debug.Log("calling ChangeDirection");

            if (!enabled || false)
            {
                heldVelocity = rb.velocity;
                rb.velocity = Vector2.zero;
            }

            canTurn = false;
            return true;
        }
        return false;
    }

    // Returns true if transform.position + direction is empty
    private bool PointClear(Vector2 direction)
    {
        LayerMask mask = ~(1 << 11) & ~(1 << 5);
        if (gameObject.CompareTag("Enemy"))
            mask = mask & ~(1 << 9);

        return Physics2D.Raycast((Vector2)transform.position + direction, Vector2.up, 0f, mask).collider == null;
    }

    public void ModifySpeed(float penalty)
    {
        walkSpeed = baseWalkSpeed - penalty;
        runSpeed = baseRunSpeed - penalty;
    }

    public bool GetCanTurn()
    {
        return canTurn;
    }

    public Vector2 GetDiscretePosition()
    {
        return nextDiscretePosition;
    }

    // Wrapper for rotator.Rotate function
    private void Rotate(int ang)
    {
        if(rotator != null)
            rotator.Rotate(ang);
    }

    /*public Rotator GetRotator()
    {
        return rotator;
    }*/

}
