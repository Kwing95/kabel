using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    public enum Item { Frag, Flash, Smoke, Gauze, Salts };
    private Dictionary<Item, int> inventory;

    // Start is called before the first frame update
    void Start()
    {
        inventory = new Dictionary<Item, int>();
        inventory.Add(Item.Frag, 0);
    }

    // Add a single item to inventory
    public void Assimilate(KeyValuePair<Item, int> i)
    {
        if (inventory.ContainsKey(i.Key))
            inventory[i.Key] += i.Value;
        else
            inventory[i.Key] = i.Value;
    }

    // Add a dictionary of items to inventory
    public void Assimilate(Dictionary<Item, int> pickup)
    {
        foreach(KeyValuePair<Item, int> i in pickup)
        {
            Assimilate(i);
        }
    }

    // Get number of an item type
    public int GetStock(Item type)
    {
        return inventory.ContainsKey(type) ? inventory[type] : 0;
    }

    // Attempts to consume an item of type; returns true if item is consumed successfully
    public bool Consume(Item type)
    {
        if(inventory.ContainsKey(type) && inventory[type] > 0)
        {
            inventory[type] = inventory[type] - 1;
            return true;
        }
        return false;
    }

    // Return Dictionary "inventory"; should only be used for Assimilate
    public Dictionary<Item, int> GetInventory()
    {
        return inventory;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

