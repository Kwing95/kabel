using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleUnit : FieldUnit
{
    private int health;
    private int focus;
    private bool real;

    // Start is called before the first frame update
    void Start()
    {
        FullHeal();
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public override void FullHeal()
    {
        health = maxHealth;
        focus = maxFocus;
    }

    public override void ResetAP()
    {
        ap = maxAP - (3 - health);
    }

    public override int GetHealth()
    {
        return health;
    }

    public override int GetFocus()
    {
        return focus;
    }
}