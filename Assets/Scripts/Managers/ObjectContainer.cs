using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectContainer : MonoBehaviour
{

    public static ObjectContainer instance;
    public GameObject enemies;
    public GameObject corpses;
    public GameObject loot;
    public GameObject wounded;
    public GameObject projectiles;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    public static List<GameObject> GetAllLoot()
    {
        List<GameObject> lootList = new List<GameObject>();
        for (int i = 0; i < instance.loot.transform.childCount; ++i)
            lootList.Add(instance.loot.transform.GetChild(i).gameObject);

        return lootList;
    }

    public static List<GameObject> GetAllProjectiles()
    {
        List<GameObject> projectileList = new List<GameObject>();
        for (int i = 0; i < instance.projectiles.transform.childCount; ++i)
            projectileList.Add(instance.projectiles.transform.GetChild(i).gameObject);

        return projectileList;
    }

    public static List<GameObject> GetAllWounded()
    {
        List<GameObject> woundedList = new List<GameObject>();
        for (int i = 0; i < instance.wounded.transform.childCount; ++i)
            woundedList.Add(instance.wounded.transform.GetChild(i).gameObject);

        return woundedList;
    }

    public static List<GameObject> GetAllCorpses()
    {
        List<GameObject> corpseList = new List<GameObject>();
        for (int i = 0; i < instance.corpses.transform.childCount; ++i)
            corpseList.Add(instance.corpses.transform.GetChild(i).gameObject);

        return corpseList;
    }

    public static List<GameObject> GetEnemiesAndWounded()
    {
        return GetAllEnemies().Concat(GetAllWounded()).ToList();
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
        List<GameObject> units = GetEnemiesAndWounded();
        units.Add(PlayerMover.instance.gameObject);

        return units;
    }

}
