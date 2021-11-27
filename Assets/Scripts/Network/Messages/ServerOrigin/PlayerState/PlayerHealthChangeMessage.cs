using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Network.Messages.ServerOrigin
{
    [Serializable]
    class PlayerHealthChangeMessage
    {
        public int clientId;
        public int currentHealth;

        public PlayerHealthChangeMessage(int clientId, int currentHealth)
        {
            this.currentHealth = currentHealth;
            this.clientId = clientId;
        }
    }
}
