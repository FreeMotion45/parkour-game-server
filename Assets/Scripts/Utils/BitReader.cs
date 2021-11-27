using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Utils
{
    class BitReader : IEnumerable<int>
    {
        private const int BITS_IN_BYTE = 8;
        private const int HEADER_BYTES = 4;

        private byte[] bytes;
        private int offset;

        private Stream stream;

        private int bits;

        public BitReader()
        {
            bytes = new byte[0];
        }

        public BitReader(byte[] bytes, int offset)
        {
            Update(bytes, offset);
        }

        public BitReader(Stream stream)
        {
            Update(stream);
        }

        public void Update(byte[] bytes, int offset)
        {
            bits = BitConverter.ToInt32(bytes, offset);
            this.bytes = bytes;
            this.offset = offset + HEADER_BYTES;
        }

        public void Update(byte[] bytes, int offset, int bits)
        {
            this.bits = bits;
            this.offset = offset;
            this.bytes = bytes;
        }

        public void Update(Stream stream)
        {
            byte[] bitCountBytes = new byte[HEADER_BYTES];
            stream.Read(bitCountBytes, 0, HEADER_BYTES);
            bits = BitConverter.ToInt32(bitCountBytes, 0);

            int bytesLeft = bits / 8;
            if (bits % 8 != 0)
                bytesLeft++;

            bytes = new byte[bytesLeft];
            stream.Read(bytes, HEADER_BYTES, bytes.Length);

            offset = 0;
        }    
        
        public int ReadNumberBits(int offset, int numberBits)
        {
            int result = 0;            
            for (int i = offset; i < numberBits; i++)
            {
                int currentByteIndex = i / BITS_IN_BYTE;
                byte currentByte = bytes[currentByteIndex + this.offset];

                int currentBitIndex = i % BITS_IN_BYTE;
                int bit = (currentByte >> currentBitIndex) & 1;

                int shift = (numberBits - offset) - (numberBits - i);
                result |= (bit << shift);
            }
            return result;
        }

        public IEnumerator<int> GetEnumerator()
        {
            for (int i = 0; i < bits; i++)
            {
                int currentByteIndex = i / BITS_IN_BYTE;
                byte currentByte = bytes[currentByteIndex + offset];

                int currentBitIndex = i % BITS_IN_BYTE;
                int bit = (currentByte >> currentBitIndex) & 1;
                yield return bit;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
