using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStatus : MonoBehaviour
{

    public ColorIndicator healthIndicator;
    public ColorIndicator focusIndicator;
    public SpriteRenderer unitOutline;
    public GameObject spriteContainer;

    public int numUnits = 3;
    public List<int> healthArray;
    public float gasDuration = 10;
    private float gasCounter = 0;
    private List<SpriteRenderer> unitSprites; 
    private bool isPlayer;
    private int healthLost = 0;
    private const int HEALTH_PER_UNIT = 3;
    // SEVERELY WOUNDED | WOUNDED | SLIGHTLY WOUNDED | HEALTHY
    private readonly List<Color> healthColors = new List<Color> { Color.clear, Color.red, new Color(1, 0.5f, 0), Color.yellow, Color.green };

    // Start is called before the first frame update
    void Start()
    {
        healthArray = new List<int>();
        for (int i = 0; i < numUnits; ++i)
            healthArray.Add(HEALTH_PER_UNIT);
        // health = HEALTH_PER_UNIT;// numUnits * HEALTH_PER_UNIT;
        CreateSprites();
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
        healthArray[unitToHeal] = (amount == -1 ? HEALTH_PER_UNIT : healthArray[unitToHeal] + amount);
        RefreshIndicators();
    }

    public int UnitsAlive()
    {
        int unitsAlive = 0;
        foreach(int unitHealth in healthArray)
            unitsAlive += unitHealth > 0 ? 1 : 0;

        return unitsAlive;
    }

    public static int IndexOfMin(List<int> list)
    {
        if (list.Count == 0)
            return -1;

        int minValue = list[0];
        int minIndex = 0;

        for(int i = 0; i < list.Count; ++i)
        {
            if(list[i] < minValue)
            {
                minValue = list[i];
                minIndex = i;
            }
        }

        return minIndex;
    }

    public void Death()
    {
        
        GameObject corpse = Instantiate(Globals.CORPSE, Grapher.RoundedVector(transform.position), Quaternion.identity);
        corpse.transform.parent = ObjectContainer.instance.corpses.transform;

        Inventory unitInventory = GetComponent<Inventory>();
        Inventory corpseInventory = corpse.GetComponent<Inventory>();
        if(unitInventory && corpseInventory)
            corpseInventory.Add(unitInventory.inventory, unitInventory.wallet);

        if (GetComponent<PlayerMover>())
        {
            gameObject.SetActive(false);
            // Debug.Log("Dead!");
            // TODO: Game Over menu
            //TransitionFader.instance.Transition(0);
        }
        else if (GetComponent<AutoMover>())
        {
            Destroy(gameObject);
        }
    }

    public void DamageHealth(int amount=1)
    {
        List<int> eligibleTargets = new List<int>();

        for(int i = 0; i < healthArray.Count; ++i)
            if(healthArray[i] > 0)
                eligibleTargets.Add(i);

        int playerDamaged = eligibleTargets[Random.Range(0, eligibleTargets.Count)];

        healthArray[playerDamaged] -= amount;
        healthLost += amount;

        if (IsDead())
            Death();

        RefreshIndicators();
    }

    public int HealthLost()
    {
        int total = 0;
        foreach (int amount in healthArray)
            total += (HEALTH_PER_UNIT - amount);

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
        if (numUnits == 1)
            unitOutline.color = healthColors[healthArray[0]];
        else if (numUnits == UnitsAlive())
            unitOutline.color = Color.green;
        else
            unitOutline.color = healthColors[UnitsAlive()];

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

}
