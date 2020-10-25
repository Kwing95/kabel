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

}
