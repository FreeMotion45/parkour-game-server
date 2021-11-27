using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Network.Messages.ServerOrigin
{
    [Serializable]
    class PlayerDeathMessage
    {
        public int clientId;

        public PlayerDeathMessage(int clientId)
        {
            this.clientId = clientId;
        }
    }
}
