using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Network.Messages.ServerOrigin.PickUp
{
    [Serializable]
    class PickUpPickedUpMessage
    {
        public int clientId;
        public int pickUpId;

        public PickUpPickedUpMessage(int clientId, int pickUpId)
        {
            this.pickUpId = pickUpId;
            this.clientId = clientId;
        }
    }
}
