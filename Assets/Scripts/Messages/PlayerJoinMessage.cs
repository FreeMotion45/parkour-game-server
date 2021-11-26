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
        public SerializableVector3 spawnPosition;
        public string playerName;
        public int transformHash;
        public int owner;

        public PlayerJoinMessage(string playerName, int owner, int transformHash, Vector3 spawnPosition)
        {
            this.playerName = playerName;
            this.spawnPosition = new SerializableVector3(spawnPosition);
            this.transformHash = transformHash;
            this.owner = owner;
        }
    }
}
