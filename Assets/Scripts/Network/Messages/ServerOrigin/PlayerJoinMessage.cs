using Assets.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Messages
{
    [Serializable]
    class PlayerJoinMessage
    {
        public string playerName;
        public int clientId;

        private float x;
        private float y;
        private float z;

        public PlayerJoinMessage(int owner, string playerName, Vector3 spawnPosition)
        {
            this.clientId = owner;
            this.playerName = playerName;
            Position = spawnPosition;
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
