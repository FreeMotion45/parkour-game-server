using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Network.Messages.ClientOrigin
{
    [Serializable]
    class InventorySlotMessage
    {
        public int slotToSelect;

        public InventorySlotMessage(int slotToSelect)
        {
            this.slotToSelect = slotToSelect;
        }
    }
}
