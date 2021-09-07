using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Collectible : MonoBehaviour
{
    public static int nonce = 0;

    public Sprite loot;
    public Sprite noLoot;

    private int id;
    public bool isGoal = false;
    private bool hasLoot = true;

    private SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        id = nonce++;
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector2.Distance(transform.position, PlayerMover.instance.transform.position) < 0.5f && hasLoot)
        {
            hasLoot = false;
            sr.color = Color.white;
            Inventory inv = GetComponent<Inventory>();
            PlayerMover.instance.GetComponent<Inventory>().Add(inv.inventory, inv.wallet);
        }
    }
    public int GetId()
    {
        return id;
    }

}
