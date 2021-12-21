using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Network.Messages.ServerOrigin.PlayerState
{
    [Serializable]
    class PlayerHitMessage
    {
        public int clientHitId;
        public int attackerId;
        public int currentHealth;
        public Vector3 bulletHitRelativeToHitPlayer;

        public PlayerHitMessage(int clientHitId, int attackerId, int currentHealth,
            Vector3 bulletHitRelativeToHitPlayer)
        {
            this.currentHealth = currentHealth;
            this.clientHitId = clientHitId;
            this.attackerId = attackerId;
            this.bulletHitRelativeToHitPlayer = bulletHitRelativeToHitPlayer;
        }
    }
}
