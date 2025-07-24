using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
/*public class Unit
{
    public string name;
    public int health;
    public Unit(int _health=3, string _name = "Unit")
    {
        name = _name;
        health = _health;
    }
}*/

public class UnitStatusLegacy : MonoBehaviour
{

    //public ColorIndicator healthIndicator;
    //public ColorIndicator focusIndicator;
    public SpriteRenderer unitOutline;
    public GameObject spriteContainer;
    public SingleBurst singleBurst;

    public int numUnits = 3;
    public List<Unit> healthArray;
    public float gasDuration = 10;
    private float gasCounter = 0;
    private List<SpriteRenderer> unitSprites; 
    private bool isPlayer;
    private int healthLost = 0;
    private const int MAX_HEALTH = 3;
    public int startingHealth = 3;
    // SEVERELY WOUNDED | WOUNDED | SLIGHTLY WOUNDED | HEALTHY
    private readonly List<string> stringStatus = new List<string> { "UNRESPONSIVE", "DYING", "WOUNDED", "HEALTHY" };
    private readonly List<Color> colors3x = new List<Color> { Color.clear, Color.red, Color.yellow, Color.green };
    private readonly List<Color> colors4x = new List<Color> { Color.clear, Color.red, new Color(1, 0.5f, 0), Color.yellow, Color.green };

    // Start is called before the first frame update
    void Start()
    {
        if (healthArray == null || healthArray.Count == 0)
        {
            healthArray = new List<Unit>();
            for (int i = 0; i < numUnits; ++i)
                healthArray.Add(new Unit(startingHealth));
        }

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
        int unitToHeal = IndexOfMin(healthArray);
        healthArray[unitToHeal].health = (amount == -1 ? MAX_HEALTH : healthArray[unitToHeal].health + amount);
        RefreshIndicators();
    }

    public int UnitsAlive()
    {
        int unitsAlive = 0;
        foreach(Unit unit in healthArray)
            unitsAlive += unit.health > 0 ? 1 : 0;

        return unitsAlive;
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

    public void Death()
    {
        AutoMover autoMover = GetComponent<AutoMover>();
        HideMover hideMover = GetComponent<HideMover>();
        Vector2 playerPos = Grapher.RoundedVector(PlayerMover.instance.transform.position);
        Vector2 position = Grapher.RoundedVector(transform.position);

        if (GetComponent<PlayerMover>())
        {
            gameObject.SetActive(false);
            Sidebar.instance.GameOver();
            // Debug.Log("Dead!");
            // TODO: Game Over menu
            //TransitionFader.instance.Transition(0);
        }
        else if(autoMover && autoMover.spawnsWounded)
        {
            GameObject woundedEnemy = Instantiate(Globals.WEAK_ENEMY, position, Quaternion.identity, ObjectContainer.instance.wounded.transform);

            woundedEnemy.GetComponent<HideMover>().lastSawPlayer = playerPos;
            Inventory oldInventory = GetComponent<Inventory>();
            Inventory newInventory = woundedEnemy.GetComponent<Inventory>();
            if (oldInventory && newInventory)
                newInventory.Add(oldInventory.inventory, oldInventory.wallet);

            // Wounded enemies make noise
            GameObject tempNoise = Instantiate(Globals.NOISE, position, Quaternion.identity);
            tempNoise.GetComponent<Noise>().Initialize(true, Globals.KNIFE_VOLUME, Noise.Source.Knife);

            Destroy(gameObject);
        }
        else if(hideMover || (autoMover && !autoMover.spawnsWounded))
        {
            GameObject corpse = Instantiate(Globals.CORPSE, position, Quaternion.identity);
            corpse.transform.parent = ObjectContainer.instance.corpses.transform;

            Inventory unitInventory = GetComponent<Inventory>();
            Inventory corpseInventory = corpse.GetComponent<Inventory>();
            if (unitInventory && corpseInventory)
                corpseInventory.Add(unitInventory.inventory, unitInventory.wallet);

            Destroy(gameObject);
        }
    }

    public void DamageHealth(int amount=1)
    {
        List<int> eligibleTargets = new List<int>();

        for(int i = 0; i < healthArray.Count; ++i)
            if(healthArray[i].health > 0)
                eligibleTargets.Add(i);

        if(eligibleTargets.Count > 0)
        {
            int playerDamaged = eligibleTargets[Random.Range(0, eligibleTargets.Count)];
            healthArray[playerDamaged].health -= amount;
        }

        if (singleBurst)
            singleBurst.Burst();

        healthLost += amount;

        if (IsDead())
            Death();

        RefreshIndicators();
    }

    public int HealthLost()
    {
        int total = 0;
        foreach (Unit amount in healthArray)
            total += (MAX_HEALTH - amount.health);

        return total;
    }

    private bool IsDead()
    {
        return UnitsAlive() == 0;
    }
    /*
    public int GetHealth()
    {
        return health;
    }*/

    private void RefreshIndicators()
    {
        if (unitOutline)
        {
            if (numUnits == 1 && healthArray.Count > 0)
                unitOutline.color = colors3x[Mathf.Clamp(healthArray[0].health, 0, colors3x.Count - 1)];
            else if (numUnits == UnitsAlive())
                unitOutline.color = Color.green;
            else
                unitOutline.color = colors4x[UnitsAlive()];
        }

        /*for(int i = 0; i < unitSprites.Count; ++i)
        {
            if(healthArray[i] >= 0)
                unitSprites[i].color = healthColors[healthArray[i]];
        }*/
        // healthIndicator.SetColor(healthArray);
        // focusIndicator.SetColor(focus);
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

    public int GetHealthLost()
    {
        return healthLost;
    }

    public string StringStatus()
    {
        string output = "";
        foreach(Unit unit in healthArray)
            output += unit.name + ": " + stringStatus[unit.health] + "\n";

        return output;
    }

}
