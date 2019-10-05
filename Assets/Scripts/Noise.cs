using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise : MonoBehaviour
{

    public bool friendly; // Friendly indicates the player made the noise
    public float hearDistance = 3.3f;
    private GameObject enemies;
    //private CircleCollider2D collider;
    private Color color;

    // Start is called before the first frame update
    void Start()
    {
        color = GetComponent<SpriteRenderer>().color;
    }

    public void Initialize(bool isFriendly, float volume)
    {
        friendly = isFriendly;
        hearDistance = volume;
        transform.localScale = (0.2f + (0.4f * volume)) * Vector2.one;

        if (friendly)
            NotifyEnemies();
    }

    public void NotifyEnemies()
    {
        enemies = EnemyList.instance.gameObject;

        for (int i = 0; i < enemies.transform.childCount; ++i)
        {
            if (Vector2.Distance(enemies.transform.GetChild(i).transform.position, transform.position) <= hearDistance)
            {
                AutoMover mover = enemies.transform.GetChild(i).GetComponent<AutoMover>();
                if (mover != null)
                {
                    mover.SetDestination(transform.position, true);
                    mover.SetChasing(true);
                }

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = transform.localScale * 1.01f;
        color.a -= 0.5f * Time.deltaTime;
        GetComponent<SpriteRenderer>().color = color;
        if (color.a <= 0)
            Destroy(gameObject);
    }
}
