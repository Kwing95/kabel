using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyList : MonoBehaviour
{

    public static EnemyList instance;
    //public static int numEnemies = 0;

    // Start is called before the first frame update
    void Start()
    {
        //numEnemies = transform.childCount;
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static List<GameObject> GetAllEnemies()
    {
        List<GameObject> enemies = new List<GameObject>();
        for (int i = 0; i < instance.transform.childCount; ++i)
            enemies.Add(instance.transform.GetChild(i).gameObject);

        return enemies;
    }

    public static List<GameObject> GetAllUnits()
    {
        List<GameObject> units = GetAllEnemies();
        units.Add(PlayerMover.instance.gameObject);
        
        return units;
    }

}
