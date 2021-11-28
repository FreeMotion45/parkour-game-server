using Assets.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Network.Messages.ServerOrigin
{
    [Serializable]
    class PlayerSpawnMessage
    {
        public int clientId;
        public SerializableVector3 position;

        public PlayerSpawnMessage(int clientId, Vector3 position)
        {
            this.clientId = clientId;
            this.position = new SerializableVector3(position);
        }
    }
}
