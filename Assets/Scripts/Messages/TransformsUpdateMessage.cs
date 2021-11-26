using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Messages
{
    [Serializable]
    class TransformsUpdateMessage
    {
        public int[] transformHashes;

        public float[] posX;
        public float[] posY;
        public float[] posZ;

        public float[] eulerX;
        public float[] eulerY;
        public float[] eulerZ;

        public TransformsUpdateMessage(IEnumerable<int> transformHashes, IEnumerable<Vector3> positions, IEnumerable<Vector3> eulers)
        {
            this.transformHashes = transformHashes.ToArray();

            posX = new float[this.transformHashes.Length];
            posY = new float[this.transformHashes.Length];
            posZ = new float[this.transformHashes.Length];

            eulerX = new float[this.transformHashes.Length];
            eulerY = new float[this.transformHashes.Length];
            eulerZ = new float[this.transformHashes.Length];

            Positions = positions.ToArray();
            Eulers = eulers.ToArray();
        }

        public Vector3[] Positions
        {
            get => Enumerable.Range(0, transformHashes.Length)
                .Select(i => new Vector3(posX[i], posY[i], posZ[i])).ToArray();
            set
            {
                for (int i = 0; i < value.Length; i++)
                {
                    posX[i] = value[i].x;
                    posY[i] = value[i].y;
                    posZ[i] = value[i].z;
                }
            }
        }

        public Vector3[] Eulers
        {
            get => Enumerable.Range(0, transformHashes.Length)
                .Select(i => new Vector3(eulerX[i], eulerY[i], eulerZ[i])).ToArray();
            set
            {

                for (int i = 0; i < value.Length; i++)
                {
                    eulerX[i] = value[i].x;
                    eulerY[i] = value[i].y;
                    eulerZ[i] = value[i].z;
                }
            }
        }
    }
}
