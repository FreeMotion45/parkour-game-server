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
    class MovePlayerMessage
    {
        public float posX;
        public float posY;
        public float posZ;

        public float rotX;
        public float rotY;
        public float rotZ;
        public float rotW;

        public MovePlayerMessage(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }

        public Vector3 Position
        {
            get => new Vector3(posX, posY, posZ);
            set
            {
                posX = value.x;
                posY = value.y;
                posZ = value.z;
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
}
