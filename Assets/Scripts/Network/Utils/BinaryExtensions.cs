using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Network.Utils
{
    public static class BinaryExtensions
    {
        public static void Write(this BinaryWriter writer, Vector3[] vecs)
        {
            writer.Write(vecs.Length);
            foreach (Vector3 v in vecs)
            {
                writer.Write(v);
            }
        }

        public static Vector3[] ReadVector3Array(this BinaryReader reader)
        {
            int count = reader.ReadInt32();
            Vector3[] vecs = new Vector3[count];
            for (int i = 0; i < count; i++)
            {
                vecs[i] = ReadVector3(reader);
            }
            return vecs;
        }

        public static void Write(this BinaryWriter writer, Quaternion[] quaternions)
        {
            foreach (Quaternion q in quaternions)
            {
                writer.Write(q);
            }
        }

        public static List<Quaternion> ReadQuaternionArray(this BinaryReader reader, int count)
        {
            List<Quaternion> quaternions = new List<Quaternion>();
            for (int i = 0; i < count; i++)
            {
                quaternions.Add(ReadQuaternion(reader));
            }
            return quaternions;
        }

        public static void Write(this BinaryWriter writer, Vector3 vec)
        {
            writer.Write(vec.x);
            writer.Write(vec.y);
            writer.Write(vec.z);
        }

        public static Vector3 ReadVector3(this BinaryReader reader)
        {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            float z = reader.ReadSingle();
            return new Vector3(x, y, z);
        }

        public static void Write(this BinaryWriter writer, Quaternion q)
        {
            writer.Write(q.x);
            writer.Write(q.y);
            writer.Write(q.z);
            writer.Write(q.w);
        }

        public static Quaternion ReadQuaternion(this BinaryReader reader)
        {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            float z = reader.ReadSingle();
            float w = reader.ReadSingle();
            return new Quaternion(x, y, z, w);
        }

        public static void WriteClientID(this BinaryWriter writer, int clientId)
        {
            writer.Write(clientId);
        }

        public static int ReadClientID(this BinaryReader reader)
        {
            return reader.ReadInt32();
        }
    }
}
