using System.Collections.Generic;

public class InventoryItem : ObjectIdentifier
{
    private readonly static Dictionary<InventoryItemType, int> _stackSize = new Dictionary<InventoryItemType, int>();

    public static int GetStackSize(InventoryItemType inventoryItemType)
    {
        return _stackSize[inventoryItemType];
    }

    public enum InventoryItemType
    {
        Gun,
        Shotgun,
        Grenade,
    }

    public InventoryItemType inventoryItemType;
}
