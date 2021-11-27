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
    struct PlayerInformation
    {        
        public int networkID;        
        public SerializableVector3 Position;
        public SerializableQuaternion Rotation;

        public PlayerInformation(int id, Vector3 position, Quaternion rotation) : this()
        {            
            networkID = id;            
            Position = new SerializableVector3(position);
            Rotation = new SerializableQuaternion(rotation);
        }
    }

    [Serializable]
    class OnJoinWorldState
    {
        public PlayerInformation[] playerInformation;

        public OnJoinWorldState(IEnumerable<PlayerInformation> playerInformation)
        {
            this.playerInformation = playerInformation.ToArray();
        }
    }
}
