using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Network.Messages.ClientOrigin.Inventory
{
    [Serializable]
    class InventorySlotMessage
    {
        public int slotIndex;

        public InventorySlotMessage(int slotIndex)
        {
            this.slotIndex = slotIndex;
        }
    }
}
