using Assets.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Messages
{
    [Serializable]
    struct PlayerInformation
    {
        public string name;
        public int networkID;
        public int transformHash;
        public SerializableVector3 Position;
        public SerializableVector3 Rotation;

        public PlayerInformation(string name, int id, int transformHash,
            SerializableVector3 position, SerializableVector3 rotation) : this()
        {
            this.name = name;
            this.networkID = id;
            this.transformHash = transformHash;
            Position = position;
            Rotation = rotation;
        }
    }

    [Serializable]
    class WorldStateMessage
    {
        public PlayerInformation[] playerInformation;

        public WorldStateMessage(IEnumerable<PlayerInformation> playerInformation)
        {
            this.playerInformation = playerInformation.ToArray();
        }
    }
}
