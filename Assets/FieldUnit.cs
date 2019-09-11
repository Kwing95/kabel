using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FieldUnit : MonoBehaviour
{

    protected int maxAP = 4;
    protected int ap = 0;

    protected int maxHealth = 3;
    protected int maxFocus = 3;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public abstract void ResetAP();
    public abstract void FullHeal();
    public abstract int GetHealth();
    public abstract int GetFocus();

    public int GetAP()
    {
        return ap;
    }

    public int GetMaxAP()
    {
        return maxAP;
    }

    public void ConsumeAP(int amount)
    {
        ap -= amount;
    }

    public void EndTurn()
    {
        ap = 0;
    }

}