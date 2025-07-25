﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class Unit
{
    public string name;
    public int health;
    public Unit(int _health=3, string _name = "Unit")
    {
        name = _name;
        health = _health;
    }
}

public class UnitStatus : MonoBehaviour
{

    public SpriteRenderer unitOutline;
    public GameObject spriteContainer;
    public SingleBurst singleBurst;

    public int numUnits = 3;
    public float gasDuration = 10;
    private float gasCounter = 0;
    private List<SpriteRenderer> unitSprites; 
    private bool isPlayer;
    public const int MAX_HEALTH_PER_UNIT = 3;
    private int maxHealth = 3;
    private int currentHealth = 3;
    public bool isWounded = false;

    // SEVERELY WOUNDED | WOUNDED | SLIGHTLY WOUNDED | HEALTHY
    private readonly List<string> stringStatus = new List<string> { "UNRESPONSIVE", "DYING", "WOUNDED", "HEALTHY" };
    private readonly List<Color> colors3x = new List<Color> { Color.clear, Color.red, Color.yellow, Color.green };
    private readonly List<Color> colors4x = new List<Color> { Color.clear, Color.red, new Color(1, 0.5f, 0), Color.yellow, Color.green };
    public readonly Color ORANGE = new Color(1, 0.5f, 0);

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = MAX_HEALTH_PER_UNIT * numUnits;
        currentHealth = maxHealth;

        if (isWounded)
            currentHealth = 1;

        // health = HEALTH_PER_UNIT;// numUnits * HEALTH_PER_UNIT;
        CreateSprites();
        isPlayer = GetComponent<PlayerMover>();
        RefreshIndicators();
    }

    // Update is called once per frame
    void Update()
    {
        gasCounter -= Time.deltaTime;
        if(gasCounter <= 0 && isPlayer)
        {

        }
    }

    public void CreateSprites()
    {
        unitSprites = new List<SpriteRenderer>();
        switch (numUnits)
        {
            case 1:
                AddUnitSprite(new Vector2(0, 0.25f));
                break;
            case 2:
                AddUnitSprite(new Vector2(-0.25f, 0));
                AddUnitSprite(new Vector2(0.25f, 0.5f));
                break;
            case 3:
                AddUnitSprite(new Vector2(-0.25f, 0.5f));
                AddUnitSprite(new Vector2(0.25f, 0.5f));
                AddUnitSprite(new Vector2(0, 0));
                break;
            case 4:
                AddUnitSprite(new Vector2(-0.25f, 0));
                AddUnitSprite(new Vector2(-0.25f, 0.5f));
                AddUnitSprite(new Vector2(0.25f, 0));
                AddUnitSprite(new Vector2(0.25f, 0.5f));
                break;
        }
    }

    public void AddUnitSprite(Vector2 offset)
    {
        unitSprites.Add(Instantiate(Globals.UNIT_SPRITE, (Vector2)spriteContainer.transform.position + offset,
            Quaternion.identity, spriteContainer.transform).GetComponent<SpriteRenderer>());
    }

    public void Heal(int amount=-1)
    {
        // Default -1 value means full health
        currentHealth = (amount == -1 ? maxHealth : currentHealth + amount);
        RefreshIndicators();
    }

    public int UnitsAlive()
    {
        return Mathf.CeilToInt((float)currentHealth / MAX_HEALTH_PER_UNIT);
    }

    public static int IndexOfMin(List<Unit> list)
    {
        if (list.Count == 0)
            return -1;

        int minValue = list[0].health;
        int minIndex = 0;

        for(int i = 0; i < list.Count; ++i)
        {
            if(list[i].health < minValue)
            {
                minValue = list[i].health;
                minIndex = i;
            }
        }

        return minIndex;
    }

    private void Death()
    {
        AutoMover autoMover = GetComponent<AutoMover>();
        HideMover hideMover = GetComponent<HideMover>();

        if (GetComponent<PlayerMover>())
        {
            gameObject.SetActive(false);
            Sidebar.instance.GameOver();
            // Debug.Log("Dead!");
            // TODO: Game Over menu
            //TransitionFader.instance.Transition(0);
        }
        else if (autoMover && autoMover.spawnsWounded && currentHealth == 0)
        {
            SpawnWounded();
            Destroy(gameObject);
        }
        // If already wounded (hideMover) or doesn't spawn wounded or has negative HP
        else if (hideMover || (autoMover && (!autoMover.spawnsWounded || currentHealth <= -1)))
        {
            SpawnCorpse();
            Destroy(gameObject);
        }
    }

    private void SpawnWounded()
    {
        Vector2 playerPos = Grapher.RoundedVector(PlayerMover.instance.transform.position);
        Vector2 position = Grapher.RoundedVector(transform.position);
        GameObject woundedEnemy = Instantiate(Globals.WEAK_ENEMY, position, Quaternion.identity, ObjectContainer.instance.wounded.transform);

        woundedEnemy.GetComponent<HideMover>().lastSawPlayer = playerPos;
        Inventory oldInventory = GetComponent<Inventory>();
        Inventory newInventory = woundedEnemy.GetComponent<Inventory>();
        if (oldInventory && newInventory)
            newInventory.Add(oldInventory.inventory, oldInventory.wallet);

        // Wounded enemies make noise
        GameObject tempNoise = Instantiate(Globals.NOISE, position, Quaternion.identity);
        tempNoise.GetComponent<Noise>().Initialize(true, Globals.KNIFE_VOLUME, Noise.Source.Knife);
    }

    private void SpawnCorpse()
    {
        Vector2 position = Grapher.RoundedVector(transform.position);
        GameObject corpse = Instantiate(Globals.CORPSE, position, Quaternion.identity);
        corpse.transform.parent = ObjectContainer.instance.corpses.transform;

        Inventory unitInventory = GetComponent<Inventory>();
        Inventory corpseInventory = corpse.GetComponent<Inventory>();
        if (unitInventory && corpseInventory)
            corpseInventory.Add(unitInventory.inventory, unitInventory.wallet);
    }

    public void DamageHealth(int amount=1)
    {
        currentHealth -= amount;

        if (singleBurst)
            singleBurst.Burst();

        if (IsDead())
            Death();

        RefreshIndicators();
    }

    public int HealthLost()
    {
        return maxHealth - currentHealth;
    }

    private bool IsDead()
    {
        return currentHealth <= 0;
    }

    private void RefreshIndicators()
    {
        if (unitOutline)
        {
            float healthDecimal = currentHealth / (float)maxHealth;
            if (healthDecimal >= 0.67f)
                unitOutline.color = Color.green;
            else if(healthDecimal >= 0.34f)
                unitOutline.color = Color.yellow;
            else
                unitOutline.color = Color.red;
        }
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
