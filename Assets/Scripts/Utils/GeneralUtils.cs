using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    static class GeneralUtils
    {
        private readonly static BitWriter bitWriter = new BitWriter();
        private readonly static BitReader bitReader = new BitReader();

        private readonly static int BIT_PRECISION = 11;
        private readonly static int QUATERNION_PRECISION_VALUE = (int)((Math.Pow(2, BIT_PRECISION) / 2) - 1);

        public static byte[] SerializeQuaternion(Quaternion q)
        {
            List<float> values = new List<float> { q.x, q.y, q.z, q.w };

            // 2 bit value
            int maxIndex = values.IndexOf(Mathf.Max(values.ToArray()));
            values.RemoveAt(maxIndex);

            int[] smallestThree = new int[3];
            for (int i = 0; i < values.Count; i++)
            {
                bool isNegative = values[i] < 0;
                int multiplier = isNegative ? -1 * QUATERNION_PRECISION_VALUE : QUATERNION_PRECISION_VALUE;
                smallestThree[i] = (int)(multiplier * values[i]);

                if (isNegative)
                    smallestThree[i] += QUATERNION_PRECISION_VALUE + 1;
            }

            bitWriter.WriteXBitsOfByte(maxIndex, 2);
            bitWriter.WriteBits(smallestThree[0], BIT_PRECISION);
            bitWriter.WriteBits(smallestThree[1], BIT_PRECISION);
            bitWriter.WriteBits(smallestThree[2], BIT_PRECISION);

            // Result bytes sized 2 + 8 * 3 = 26 bytes.
            return bitWriter.GetByteArrayAndClear();
        }

        public static Quaternion DeserializeQuaternion(byte[] b)
        {
            bitReader.Update(b, 0, 26);
            int maxIndex = bitReader.ReadNumberBits(0, 2);

            float val1 = ReadCompressedQuaternionValue(bitReader.ReadNumberBits(2, BIT_PRECISION));
            float val2 = ReadCompressedQuaternionValue(bitReader.ReadNumberBits(2 + BIT_PRECISION, 2 + BIT_PRECISION * 2));
            float val3 = ReadCompressedQuaternionValue(bitReader.ReadNumberBits(2 + BIT_PRECISION * 2, 2 + BIT_PRECISION * 3));
            float reconstructedVal = Mathf.Sqrt(1 - val1 * val1 - val2 * val2 - val3 * val3);

            // X
            if (maxIndex == 0)
                return new Quaternion(reconstructedVal, val1, val2, val3);

            // Y
            if (maxIndex == 1)
                return new Quaternion(val1, reconstructedVal, val2, val3);

            // Z
            if (maxIndex == 2)
                return new Quaternion(val1, val2, reconstructedVal, val3);

            // W
            return new Quaternion(val1, val2, val3, reconstructedVal);
        }

        private static float ReadCompressedQuaternionValue(int val)
        {
            bool isNeg = false;

            if (val > QUATERNION_PRECISION_VALUE - 1)
            {
                val -= QUATERNION_PRECISION_VALUE;
                isNeg = true;
            }

            float approx = val * 1f / QUATERNION_PRECISION_VALUE;
            if (isNeg)
                approx *= -1;

            return approx;
        }
    }
}
