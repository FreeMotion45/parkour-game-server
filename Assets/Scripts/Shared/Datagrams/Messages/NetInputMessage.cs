using Assets.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Shared.Datagrams.Messages
{
    [Serializable]
    class NetInputMessage
    {
        public byte[] inputBits;
        public int transformHash;
        public bool controllingRotation;
        public SerializableVector3 eulerAngles;

        public NetInputMessage(byte[] bytes, int transformHash,
            bool controllingRotation = false, Vector3 eulerAngles = default)
        {
            this.inputBits = bytes;
            this.transformHash = transformHash;
            this.controllingRotation = controllingRotation;
            this.eulerAngles = new SerializableVector3(eulerAngles);
        }
    }
}
