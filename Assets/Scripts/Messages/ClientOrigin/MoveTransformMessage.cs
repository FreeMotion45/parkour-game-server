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

        public float eulerX;
        public float eulerY;
        public float eulerZ;

        public MoveTransformMessage(int transformHash, Vector3 position, Vector3 eulerAngles)
        {
            this.transformHash = transformHash;
            Position = position;
            EulerAngles = eulerAngles;
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

        public Vector3 EulerAngles
        {
            get => new Vector3(eulerX, eulerY, eulerZ);
            set
            {
                eulerX = value.x;
                eulerY = value.y;
                eulerZ = value.z;
            }
        }
    }
}
