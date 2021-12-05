using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Network.Messages.ServerOrigin.PlayerState
{
    [Serializable]
    class PlayerKillMessage
    {
        public int deadClientId;
        public int killerId;

        public PlayerKillMessage(int clientId, int killerId)
        {
            this.deadClientId = clientId;
            this.killerId = killerId;
        }
    }
}
