using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Network.Messages.ServerOrigin.PlayerState
{
    [Serializable]
    class PlayerHitMessage
    {
        public int clientHitId;
        public int attackerId;
        public int currentHealth;        

        public PlayerHitMessage(int clientHitId, int attackerId, int currentHealth)
        {
            this.currentHealth = currentHealth;
            this.clientHitId = clientHitId;
            this.attackerId = attackerId;
        }
    }
}
