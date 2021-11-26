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
    public class NetAbsoluteTransform
    {
        public SerializableVector3 position;
        public SerializableVector3 eulerAngles;
        public int transformHash;

        public NetAbsoluteTransform(Vector3 position, Vector3 rotation, int transformHash)
        {
            this.position = new SerializableVector3(position);
            this.eulerAngles = new SerializableVector3(rotation);
            this.transformHash = transformHash;
        }
    }
}
