using Assets.Scripts.Server;
using Assets.Scripts.ServerLogic;
using UnityEngine;

class ServerInventory : Inventory
{    
    public float dropForce = 150f;

    public bool DropItemOfSlot(int slot, out GameObject droppedObject)
    {
        droppedObject = null;
        bool success = base.TryDropItemOfSlot(slot, out InventorySlot droppedSlot);
        
        if (!success) return success;

        InventoryItem droppedItem = droppedSlot.item;
        droppedObject = droppedItem.gameObject;
        if (droppedSlot.count > 0)
        {
            droppedObject = Instantiate(droppedSlot.item.gameObject);
        }

        Rigidbody itemRb;
        if (!droppedItem.TryGetComponent(out itemRb))
        {
            itemRb = droppedObject.AddComponent<Rigidbody>();
            itemRb.interpolation = RigidbodyInterpolation.Interpolate;
        }
        else
        {
            itemRb.isKinematic = false;
        }

        NetTransform.RegisterNewNetObject(droppedObject);
        PeriodicalPlayerInformationSender.Instance.Add(droppedObject);
        PeriodicalPlayerInformationSender.Instance.SendTransformsUpdate();

        droppedObject.SetActive(true);
        droppedObject.transform.SetParent(null);
        Vector3 worldDropOrigin = playerCamera.TransformPoint(droppedItem.positionRelativeToCamera);
        droppedObject.transform.position = worldDropOrigin;

        Vector3 dropDirection = playerCamera.forward;
        itemRb.AddForce(dropDirection * dropForce);
        itemRb.AddTorque(new Vector3(Random.value, Random.value, Random.value).normalized * 0.01f * dropForce);

        return success;
    }
}
