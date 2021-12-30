using Assets.Scripts.Network.Messages.ClientOrigin;
using Assets.Scripts.Network.Messages.ClientOrigin.Inventory;
using Assets.Scripts.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Network.Messages.ServerOrigin.Inventory
{
    class ServerInventorySlotMessage : InventorySlotMessage
    {
        public int senderId;

        public ServerInventorySlotMessage(BaseNetworkChannel senderChannel, int slotIndex)
            : base(slotIndex)
        {
            senderId = senderChannel.ChannelID;
        }

        public ServerInventorySlotMessage(int senderId, int slotIndex)
            : base(slotIndex)
        {
            this.senderId = senderId;
        }
    }
}
