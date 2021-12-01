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

        private float x;
        private float y;
        private float z;

        public PlayerSpawnMessage(int clientId, Vector3 position)
        {
            this.clientId = clientId;
            Position = position;
        }

        public Vector3 Position
        {
            get => new Vector3(x, y, z);
            set
            {
                x = value.x;
                y = value.y;
                z = value.z;
            }
        }
    }
}
