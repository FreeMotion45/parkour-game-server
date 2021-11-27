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

        // 2 + 1 + 12 * 3 = 39. Sadly, we have 1 unutilized bit...
        private readonly static int BIT_PRECISION = 12;
        private readonly static int QUATERNION_PRECISION_VALUE = (int)((Math.Pow(2, BIT_PRECISION) / 2) - 1);

        public static byte[] SerializeQuaternion(Quaternion q)
        {
            List<float> values = new List<float> { q.x, q.y, q.z, q.w };

            // 2 bit value
            int maxIndex = 0;
            for (int i = 1; i < values.Count; i++)
                maxIndex = Mathf.Abs(values[maxIndex]) > Mathf.Abs(values[i]) ? maxIndex : i;

            int reconstrucedValSignBit = values[maxIndex] >= 0 ? 0 : 1;
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
            bitWriter.WriteBit(reconstrucedValSignBit);
            bitWriter.WriteBits(smallestThree[0], BIT_PRECISION);
            bitWriter.WriteBits(smallestThree[1], BIT_PRECISION);
            bitWriter.WriteBits(smallestThree[2], BIT_PRECISION);

            // Result bytes sized 2 + 1 + 12 * 3 = 39 bytes.
            return bitWriter.GetByteArrayAndClear();
        }

        public static Quaternion DeserializeQuaternion(byte[] b)
        {
            bitReader.Update(b, 0, 2 + 1 + 3 * BIT_PRECISION);
            int maxIndex = bitReader.ReadNumberBits(0, 2);
            int reconstructedSignBit = bitReader.ReadNumberBits(2, 1);

            float val1 = ReadCompressedQuaternionValue(bitReader.ReadNumberBits(3, BIT_PRECISION));
            float val2 = ReadCompressedQuaternionValue(bitReader.ReadNumberBits(3 + BIT_PRECISION, BIT_PRECISION));
            float val3 = ReadCompressedQuaternionValue(bitReader.ReadNumberBits(3 + BIT_PRECISION * 2, BIT_PRECISION));
            float reconstructedVal = Mathf.Sqrt(1 - val1 * val1 - val2 * val2 - val3 * val3);

            if (reconstructedSignBit == 1)
                reconstructedVal *= -1;

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
                val -= QUATERNION_PRECISION_VALUE - 1;
                isNeg = true;
            }

            float approx = val * 1f / QUATERNION_PRECISION_VALUE;
            if (isNeg)
                approx *= -1;

            return approx;
        }
    }
}
