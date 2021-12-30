using Assets.Scripts.Network.Messages.ClientOrigin.Inventory;
using Assets.Scripts.Network.Messages.ServerOrigin.Inventory;
using Assets.Scripts.ServerLogic;
using UnityEngine;
using UnityMultiplayer.Shared.Networking;
using UnityMultiplayer.Shared.Networking.Datagrams;
using UnityMultiplayer.Shared.Networking.Datagrams.Handling;

namespace Assets.Scripts.Network.Handlers
{
    class DropItemOfInventorySlotHandler : BaseBehaviourDatagramHandler
    {
        public override void Handle(DatagramHolder deserializedDatagram, NetworkChannel networkChannel)
        {
            InventorySlotMessage msg = (InventorySlotMessage)deserializedDatagram.Data;
            ServerInventory inventory = GamePlayers.GetComponent<Transform>(networkChannel).Find("Inventory").GetComponent<ServerInventory>();
            bool droppedItemFromSlot = inventory.DropItemOfSlot(msg.slotIndex, out GameObject droppedItem);
            if (droppedItemFromSlot)
            {
                ServerInventoryDropItemMessage forwaded = new ServerInventoryDropItemMessage(networkChannel, msg.slotIndex, NetTransform.objectHash[droppedItem]);
                GamePlayers.Publish(forwaded, DatagramType.InventoryDropSlotConfirm);
            }
        }
    }
}
