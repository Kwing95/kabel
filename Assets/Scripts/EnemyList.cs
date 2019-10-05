using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyList : MonoBehaviour
{

    public static EnemyList instance;
    //public static int numEnemies = 0;
    private static int enemiesMoving = 0;

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

    public static void EnemyTurn()
    {
        //enemiesMoving = numEnemies;

        for (int i = 0; i < instance.transform.childCount; ++i)
        {
            enemiesMoving += 1;
            instance.transform.GetChild(i).GetComponent<FieldUnit>().ResetAP();
        }
            
    }

    public static void SetEnemiesMoving(bool increase)
    {
        enemiesMoving += increase ? 1 : -1;

        // This should only happen when decreasing
        if(enemiesMoving == 0)
        {
            PlayerMover.instance.GetComponent<FieldUnit>().ResetAP();
            MenuNode.RefreshMenu();
        }
            
    }

}
