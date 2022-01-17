using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Inventory : MonoBehaviour
{
    [Serializable]
    public enum ItemType { Frag, Distract, Gauze, Gun };
    [Serializable]
    public class InventoryEntry
    {
        public InventoryEntry(ItemType _type, int _quantity)
        {
            type = _type;
            quantity = _quantity;
        }
        public ItemType type;
        public int quantity;
    }
    
    public List<InventoryEntry> inventory;
    public int wallet = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (inventory == null)
            inventory = new List<InventoryEntry>();
    }

    public void Add(InventoryEntry newItem)
    {
        foreach(InventoryEntry item in inventory)
            if(item.type == newItem.type)
            {
                item.quantity += newItem.quantity;
                return;
            }
    }

    // Add a dictionary of items to inventory
    public void Add(List<InventoryEntry> items, int _wallet)
    {
        wallet += _wallet;
        foreach(InventoryEntry item in items)
            Add(item);
    }

    // Get number of an item type
    public int GetQuantity(ItemType type)
    {
        foreach (InventoryEntry item in inventory)
            if (item.type == type)
                return item.quantity;

        return 0;
    }

    // Attempts to consume an item of type; returns true if item is consumed successfully
    public bool Consume(ItemType type)
    {
        foreach (InventoryEntry item in inventory)
            if (item.type == type && item.quantity > 0)
            {
                item.quantity -= 1;
                return true;
            }

        return false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

