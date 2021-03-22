using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStatus : MonoBehaviour
{

    public ColorIndicator healthIndicator;
    public ColorIndicator focusIndicator;

    public int maxHealth = 3;
    public int maxFocus = 3;
    public float gasDuration = 10;
    private int health;
    private int focus;
    private float gasCounter = 0;
    private bool isPlayer;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        focus = maxFocus;
        isPlayer = GetComponent<PlayerMover>();
    }

    // Update is called once per frame
    void Update()
    {
        gasCounter -= Time.deltaTime;
        if(gasCounter <= 0 && isPlayer)
        {

        }
    }

    public void Heal(int amount=1)
    {
        health += amount;
        RefreshIndicators();
    }

    public void Death()
    {
        Instantiate(Globals.CORPSE, transform.position, Quaternion.identity);
        
        if (GetComponent<PlayerMover>())
        {
            gameObject.SetActive(false);
            TransitionFader.instance.Transition(0);
        }
        else if (GetComponent<AutoMover>())
        {
            Destroy(gameObject);
        }
    }

    public void DamageHealth(int amount=1)
    {
        health -= amount;
        if (health <= 0)
            Death();

        RefreshIndicators();
    }

    public void DamageFocus(int amount=1)
    {
        if (focus <= 0)
            DamageHealth(amount);
        else
            focus -= amount;

        RefreshIndicators();
    }

    public int HealthLost()
    {
        return maxHealth - health;
    }

    public int FocusLost()
    {
        return maxFocus - focus;
    }

    public int GetHealth()
    {
        return health;
    }

    public int GetFocus()
    {
        return focus;
    }

    private void RefreshIndicators()
    {
        healthIndicator.SetColor(health);
        focusIndicator.SetColor(focus);
    }

    public void InflictGas()
    {
        gasCounter = gasDuration;
        if (isPlayer)
        {
            // visionRadius.SetGassed(true);
            // menu can query action button for enabled?
        }
    }

    public bool IsGassed()
    {
        return gasCounter > 0;
    }

}
