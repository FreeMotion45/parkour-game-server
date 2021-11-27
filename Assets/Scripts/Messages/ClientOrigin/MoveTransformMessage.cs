using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Messages.ClientOrigin
{
    [Serializable]
    class MoveTransformMessage
    {
        public int transformHash;

        public float posX;
        public float posY;
        public float posZ;

        public float x;
        public float y;
        public float z;
        public float w;

        public MoveTransformMessage(int transformHash, Vector3 position, Quaternion rotation)
        {
            this.transformHash = transformHash;
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
