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

        private float x;
        private float y;
        private float z;

        private float rotX;
        private float rotY;
        private float rotZ;
        private float rotW;

        public PlayerInformation(int id, Vector3 position, Quaternion rotation) : this()
        {
            networkID = id;
            Position = position;
            Rotation = rotation;
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

        public Quaternion Rotation
        {
            get => new Quaternion(rotX, rotY, rotZ, rotW);
            set
            {
                rotX = value.x;
                rotY = value.y;
                rotZ = value.z;
                rotW = value.w;
            }
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
