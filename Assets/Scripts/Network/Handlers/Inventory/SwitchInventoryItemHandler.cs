using Assets.Scripts.Network.Messages.ClientOrigin.Inventory;
using Assets.Scripts.Network.Messages.ServerOrigin.Inventory;
using Assets.Scripts.ServerLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityMultiplayer.Shared.Networking;
using UnityMultiplayer.Shared.Networking.Datagrams;
using UnityMultiplayer.Shared.Networking.Datagrams.Handling;

namespace Assets.Scripts.Network.Handlers
{
    class SwitchInventoryItemHandler : BaseBehaviourDatagramHandler
    {
        public override void Handle(DatagramHolder deserializedDatagram, NetworkChannel networkChannel)
        {
            InventorySlotMessage msg = (InventorySlotMessage)deserializedDatagram.Data;
            ServerInventory inventory = GamePlayers.GetComponent<Transform>(networkChannel).Find("Inventory").GetComponent<ServerInventory>();
            inventory.EquipItemOfSlot(msg.slotIndex);

            DatagramHolder forwarded = new DatagramHolder(DatagramType.InventorySelectSlotConfirm,
                new ServerInventorySlotMessage(networkChannel, msg.slotIndex));
            GamePlayers.Publish(forwarded);
        }
    }
}
