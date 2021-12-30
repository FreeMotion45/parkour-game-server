using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public class InventorySlot
    {
        public InventoryItem item;
        public int count;

        public InventorySlot(InventoryItem item, int count)
        {
            this.item = item;
            this.count = count;
        }

        public InventoryItem GetDroppedObject()
        {
            if (count == 0)
            {
                return item;
            }

            return Instantiate(item.gameObject).GetComponent<InventoryItem>();
        }
    }

    public InventorySlot selectedSlot;

    public Transform playerCamera;

    protected List<InventorySlot> inventory = new List<InventorySlot>();

    void OnEnable()
    {
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out InventoryItem inventoryItem))
            {
                AddItem(inventoryItem);
            }
        }
    }

    public virtual void AddItem(GameObject item)
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

    public void AddItem(InventoryItem inventoryItem)
    {
        AddItem(inventoryItem.gameObject);
    }

    /// <summary>
    /// Equip item of slot index.
    /// </summary>
    /// <param name="slot"></param>
    public virtual void EquipItemOfSlot(int slot)
    {
        if (HasSlot(slot))
        {
            HideCurrentInventoryItem();
            selectedSlot = inventory[slot];
            ShowCurrentInventoryItem();
        }
        else
        {
            Debug.LogWarning(name + " tried to EQUIP at slot " + slot + " but it is empty.");
        }
    }

    /// <summary>
    /// Drop item at slot index.
    /// </summary>
    /// <param name="slot"></param>
    public virtual bool TryDropItemOfSlot(int slot, out InventorySlot droppedSlot)
    {
        if (HasSlot(slot))
        {
            droppedSlot = inventory[slot];
            droppedSlot.count -= 1;
            if (droppedSlot.count == 0)
            {
                selectedSlot = null;
                inventory.RemoveAt(slot);
            }
            return true;
        }
        else
        {
            Debug.LogWarning(name + " tried to DROP at slot " + slot + " but it is empty.");
            droppedSlot = null;
            return false;
        }
    }

    /// <summary>
    /// Checks if the slot index exists.
    /// </summary>
    /// <param name="slot"></param>
    public bool HasSlot(int slot)
    {
        return slot <= inventory.Count - 1;
    }

    public int GetSlotIndex(InventorySlot slot)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i] == slot)
            {
                return i;
            }
        }

        return -1;
    }

    private void ShowCurrentInventoryItem()
    {
        selectedSlot.item.gameObject.SetActive(true);
        Transform itemTransform = selectedSlot.item.transform;
        itemTransform.SetParent(playerCamera);

        InventoryItem inventoryItem = itemTransform.GetComponent<InventoryItem>();
        itemTransform.localPosition = inventoryItem.positionRelativeToCamera;
        itemTransform.localRotation = Quaternion.Euler(inventoryItem.defaultRotation);
    }

    private void HideCurrentInventoryItem()
    {
        // If any weapon is equipped
        if (selectedSlot != null)
        {
            Transform inventoryItemToHide = selectedSlot.item.transform;
            inventoryItemToHide.SetParent(transform);
            inventoryItemToHide.gameObject.SetActive(false);
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
