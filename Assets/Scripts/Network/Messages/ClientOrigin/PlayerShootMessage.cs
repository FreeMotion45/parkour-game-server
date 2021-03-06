using Assets.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Messages.ClientOrigin
{
    [Serializable]
    class PlayerShootMessage
    {
        public int clientId;

        public float x;
        public float y;
        public float z;
        public float w;

        public PlayerShootMessage(int clientId, Quaternion rotation)
        {
            this.clientId = clientId;
            Rotation = rotation;
        }

        public Quaternion Rotation
        {
            get => new Quaternion(x, y, z, w);
            set
            {
                x = value.x;
                y = value.y;
                z = value.z;
                w = value.w;
            }
        }
    }
}
