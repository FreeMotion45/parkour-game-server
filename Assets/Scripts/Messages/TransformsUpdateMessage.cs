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

        public float[] x;
        public float[] y;
        public float[] z;
        public float[] w;

        public TransformsUpdateMessage(IEnumerable<int> transformHashes, IEnumerable<Vector3> positions, IEnumerable<Quaternion> rotations)
        {
            this.transformHashes = transformHashes.ToArray();

            posX = new float[this.transformHashes.Length];
            posZ = new float[this.transformHashes.Length];
            posY = new float[this.transformHashes.Length];

            x = new float[this.transformHashes.Length];
            y = new float[this.transformHashes.Length];
            z = new float[this.transformHashes.Length];
            w = new float[this.transformHashes.Length];

            Positions = positions.ToArray();
            Rotations = rotations.ToArray();
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

        public Quaternion[] Rotations
        {
            get => Enumerable.Range(0, transformHashes.Length)
                .Select(i => new Quaternion(x[i], y[i], z[i], w[i])).ToArray();
            set
            {
                for (int i = 0; i < value.Length; i++)
                {
                    x[i] = value[i].x;
                    y[i] = value[i].y;
                    z[i] = value[i].z;
                    w[i] = value[i].w;
                }
            }
        }
    }
}
