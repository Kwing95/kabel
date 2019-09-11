using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyList : MonoBehaviour
{

    public static EnemyList instance;
    private static int enemiesMoving = 0;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {

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
