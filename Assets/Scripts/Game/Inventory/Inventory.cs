using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{    
    class InventorySlot
    {        
        public InventoryItem item;
        public int count;

        public InventorySlot(InventoryItem item, int count)
        {            
            this.item = item;
            this.count = count;
        }
    }

    public GameObject itemInHand;

    private List<InventorySlot> inventory = new List<InventorySlot>();
    
    public void AddItem(GameObject item)
    {
        if (item.TryGetComponent(out InventoryItem inventoryItem))
        {
            InventorySlot[] freeSlots = inventory
                .Where(slot => CanAppendToInventory(inventoryItem, slot))
                .ToArray();
            if (freeSlots.Length > 0)
            {
                freeSlots[0].count += 1;
            }
            else
            {
                inventory.Add(new InventorySlot(inventoryItem, 1));
            }
        }
    }

    /// <summary>
    /// Equip item of slot index (1 based indexing).
    /// </summary>
    /// <param name="slot"></param>
    public void EquipItemOfSlot(int slot)
    {
        if (inventory.Count <= slot)
        {
            itemInHand = inventory[slot].item.gameObject;
        }
        else
        {
            Debug.LogWarning(name + " tried to EQUIP at slot " + slot + " but it is empty.");
        }
    }

    /// <summary>
    /// Drop item at slot index (1 based indexing).
    /// </summary>
    /// <param name="slot"></param>
    public void DropItemOfSlot(int slot)
    {
        if (inventory.Count <= slot)
        {
            inventory[slot].count -= 1;
        }
        else
        {
            Debug.LogWarning(name + " tried to DROP at slot " + slot + " but it is empty.");
        }
    }

    /// <summary>
    /// Helper method for LINQ thats returns whether an item can be appended to an existing slot.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="slot"></param>
    /// <returns>bool</returns>
    private bool CanAppendToInventory(InventoryItem item, InventorySlot slot)
    {
        if (item.inventoryItemType == slot.item.inventoryItemType)
        {
            if (slot.count < InventoryItem.GetStackSize(item.inventoryItemType))
            {
                return true;
            }
        }

        return false;
    }
}
