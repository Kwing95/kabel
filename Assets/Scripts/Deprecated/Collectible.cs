using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Collectible : MonoBehaviour
{

    public Sprite loot;
    public Sprite noLoot;

    public bool isGoal = false;
    private bool hasLoot = true;

    private GridMover player;
    private AudioSource kaching;
    private SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (isGoal)
            transform.position = PlayerMover.instance.transform.position;
        kaching = GetComponent<AudioSource>();
        player = PlayerMover.instance.GetComponent<GridMover>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector2.Distance(transform.position, PlayerMover.instance.transform.position) < 0.5f && hasLoot)
        {
            sr.sprite = noLoot;
        }
    }

}
