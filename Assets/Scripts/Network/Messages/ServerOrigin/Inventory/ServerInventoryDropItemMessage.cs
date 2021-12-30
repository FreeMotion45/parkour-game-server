using Assets.Scripts.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Network.Messages.ServerOrigin.Inventory
{
    class ServerInventoryDropItemMessage : ServerInventorySlotMessage
    {
        public int droppedItemTransformHash;

        public ServerInventoryDropItemMessage(BaseNetworkChannel senderChannel, int slotIndex, int droppedItemTransformHash)
            : base(senderChannel, slotIndex)
        {
            this.droppedItemTransformHash = droppedItemTransformHash;
        }

        public ServerInventoryDropItemMessage(int senderId, int slotIndex, int droppedItemTransformHash)
            : base(senderId, slotIndex)
        {
            this.droppedItemTransformHash = droppedItemTransformHash;
        }
    }
}
