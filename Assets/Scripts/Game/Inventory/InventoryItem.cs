using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : ObjectIdentifier
{
    private readonly static Dictionary<InventoryItemType, int> stackSize = new Dictionary<InventoryItemType, int>()
    {
        { InventoryItemType.Gun, 1 },
        { InventoryItemType.Grenade, 3 },
        { InventoryItemType.Shotgun, 1 },
    };

    public static int GetStackSize(InventoryItemType inventoryItemType)
    {
        return stackSize[inventoryItemType];
    }

    public enum InventoryItemType
    {
        Gun,
        Shotgun,
        Grenade,
    }

    public InventoryItemType inventoryItemType;

    [Space]
    public Vector3 positionRelativeToCamera;
    public Vector3 defaultRotation;
}
