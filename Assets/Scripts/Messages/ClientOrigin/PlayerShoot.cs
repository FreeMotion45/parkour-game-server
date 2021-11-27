using Assets.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Messages.ClientOrigin
{
    [Serializable]
    class PlayerShoot
    {
        public byte[] quaternionBytes;

        public PlayerShoot(Quaternion rotation)
        {
            Rotation = rotation;
        }

        public Quaternion Rotation
        {
            get => GeneralUtils.DeserializeQuaternion(quaternionBytes);
            set => quaternionBytes = GeneralUtils.SerializeQuaternion(value);
        }
    }
}
