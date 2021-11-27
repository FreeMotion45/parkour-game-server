using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    [Serializable]
    class SerializableQuaternion
    {
        private byte[] bytes;

        public SerializableQuaternion(Quaternion quaternion)
        {
            Rotation = quaternion;
        }

        public Quaternion Rotation
        {
            get => GeneralUtils.DeserializeQuaternion(bytes);
            set => bytes = GeneralUtils.SerializeQuaternion(value);
        }
    }
}
