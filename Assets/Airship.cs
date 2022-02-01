using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airship : MonoBehaviour
{

    private Rigidbody2D rb;

    public float leftBoundary = 0;
    public float rightBoundary = 100;
    public float attackRange = 10;
    public float attackCooldown = 3;
    public float speed = 4;
    private float attackTimer = 0;
    private int directionMultiplier = -1;
    private Vector2 storedVelocity = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.position = new Vector2(rightBoundary, PlayerMover.instance.transform.position.y);
    }

    private void OnDisable()
    {
        storedVelocity = rb.velocity;
        rb.velocity = Vector2.zero;
    }

    private void OnEnable()
    {
        rb.velocity = storedVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector2.Distance(rb.position, PlayerMover.instance.transform.position);
        float dy = PlayerMover.instance.transform.position.y - rb.position.y;
        rb.velocity = new Vector2(Mathf.Clamp(distance / 2f, 1, 4) * directionMultiplier, Mathf.Clamp(dy, -0.5f, 0.5f));

        if(transform.position.x < leftBoundary || transform.position.x > rightBoundary)
        {
            ChangeDirections();
        }

        attackTimer -= Time.deltaTime;
        if(attackTimer <= 0 && distance < attackRange)
        {
            Attack();
            attackTimer = attackCooldown;
        }
    }

    public void ChangeDirections()
    {
        directionMultiplier *= -1;
        rb.position = new Vector2(rb.position.x + directionMultiplier, PlayerMover.instance.transform.position.y);
        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + 180);
    }

    public void Attack()
    {
        ActionManager.SpawnProjectile(gameObject, PlayerMover.instance.transform.position, Projectile.Type.Frag);
    }
}
