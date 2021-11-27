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
    class MoveTransformMessage
    {
        public int transformHash;

        public float posX;
        public float posY;
        public float posZ;

        public byte[] quaternionBytes;

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
            get => GeneralUtils.DeserializeQuaternion(quaternionBytes);
            set => quaternionBytes = GeneralUtils.SerializeQuaternion(value);
        }
    }
}
