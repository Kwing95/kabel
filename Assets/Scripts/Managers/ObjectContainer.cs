using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectContainer : MonoBehaviour
{

    public static ObjectContainer instance;
    public GameObject enemies;
    public GameObject corpses;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    public static List<GameObject> GetAllCorpses()
    {
        List<GameObject> corpseList = new List<GameObject>();
        for (int i = 0; i < instance.corpses.transform.childCount; ++i)
            corpseList.Add(instance.corpses.transform.GetChild(i).gameObject);

        return corpseList;
    }

    public static List<GameObject> GetAllEnemies()
    {
        List<GameObject> enemyList = new List<GameObject>();
        for (int i = 0; i < instance.enemies.transform.childCount; ++i)
            enemyList.Add(instance.enemies.transform.GetChild(i).gameObject);

        return enemyList;
    }

    public static List<GameObject> GetAllUnits()
    {
        List<GameObject> units = GetAllEnemies();
        units.Add(PlayerMover.instance.gameObject);

        return units;
    }

}
