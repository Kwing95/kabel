using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise : MonoBehaviour
{
    public enum Source { Distract, Grenade, Gun, Footsteps, Knife, Ally };
    private Source source;
    private Vector2 secondaryPoint;
    private bool madeByPlayer; // Friendly indicates the player made the noise
    public float hearDistance = 3.3f;
    private GameObject enemies;
    //private CircleCollider2D collider;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Enemies may investigate a secondary position, such as where a grenade was thrown from vs where it landed
    public void Initialize(bool _madeByPlayer, float volume, Source noiseSource, Vector2 secondaryPosition)
    {
        secondaryPoint = secondaryPosition;
        Initialize(_madeByPlayer, volume, noiseSource);
    }

    public void Initialize(bool _madeByPlayer, float volume, Source noiseSource)
    {
        madeByPlayer = _madeByPlayer;
        source = noiseSource;
        hearDistance = volume;
        transform.localScale = (0.2f + (0.4f * volume)) * Vector2.one;

        NotifyEnemies();
    }

    private void Initialize(bool isFriendly, float volume)
    {
        madeByPlayer = isFriendly;
        hearDistance = volume;
        transform.localScale = (0.2f + (0.4f * volume)) * Vector2.one;

        if (madeByPlayer)
            NotifyEnemies();
    }

    private void NotifyEnemies()
    {
        enemies = ObjectContainer.instance.enemies;

        for (int i = 0; i < enemies.transform.childCount; ++i)
        {
            Transform enemy = enemies.transform.GetChild(i);
            if (Vector2.Distance(enemy.transform.position, transform.position) <= hearDistance)
            {
                AutoMover autoMover = enemy.GetComponent<AutoMover>();
                if (autoMover)
                    autoMover.SoundToPosition(Grapher.RoundedVector(transform.position), madeByPlayer, source, secondaryPoint);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
