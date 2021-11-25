using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Utils
{
    class BitWriter
    {
        private const int BITS_IN_BYTE = 8;
        private const int HEADER_BYTES = 8;

        private readonly List<byte> bits;
        private byte currentByte;
        private int bitIndexInByte;

        public BitWriter()
        {
            bits = new List<byte>();
        }

        public int BitsWritten => bits.Count * BITS_IN_BYTE + bitIndexInByte;

        public void WriteBit(byte bit)
        {
            if (bit != 0 && bit != 1)
            {
                throw new ArgumentException("A bit can only be either 1 or 0!");
            }

            if (bit == 1)
            {
                currentByte |= (byte)(1 << bitIndexInByte);
            }

            bitIndexInByte++;

            if (bitIndexInByte == BITS_IN_BYTE)
            {
                bits.Add(currentByte);
                currentByte = 0;
                bitIndexInByte = 0;
            }
        }

        public byte[] GetByteArray()
        {
            return bits.ToArray();
        }

        public byte[] GetByteArraySerialized()
        {
            byte[] bytes = new byte[HEADER_BYTES + bits.Count];
            BitConverter.GetBytes(BitsWritten).CopyTo(bytes, 0);
            bits.CopyTo(bytes, HEADER_BYTES);
            return bytes;
        }

        public void SerializeWrite(Stream stream)
        {
            byte[] bitsWrittenBytes = BitConverter.GetBytes(BitsWritten);
            stream.Write(bitsWrittenBytes, 0, bitsWrittenBytes.Length);
            stream.Write(bits.ToArray(), 0, bits.Count);
        }

        public void Clear()
        {
            bits.Clear();
            bitIndexInByte = 0;
            currentByte = 0;
        }
    }
}
