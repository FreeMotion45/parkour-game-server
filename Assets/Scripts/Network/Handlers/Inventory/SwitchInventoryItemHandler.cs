using Assets.Scripts.Network.Messages.ClientOrigin;
using Assets.Scripts.Network.Messages.ServerOrigin.Inventory;
using Assets.Scripts.ServerLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Inventory inventory = PlayerDatabase.GetComponent<Inventory>(networkChannel);
            inventory.EquipItemOfSlot(msg.slotToSelect);

            DatagramHolder forwarded = new DatagramHolder(DatagramType.InventorySelectSlot,
                new ServerInventorySlotMessage(networkChannel, msg.slotToSelect));
            PlayerDatabase.Publish(forwarded);
        }
    }
}
