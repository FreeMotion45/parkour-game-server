using Assets.Scripts.Shared.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Shared.Datagrams.Messages
{
    public enum NetworkMovement
    {
        OnDemand,
        Interpolated,
    }

    [Serializable]
    class UpdateNetTransformMessage
    {
        public int hash;

        public float posx;
        public float posy;
        public float posz;

        public float rotx;
        public float roty;
        public float rotz;

        public NetworkMovement networkMovement;

        public UpdateNetTransformMessage(Vector3 position, Vector3 eulerAngles,
            NetworkMovement networkMovement, int hash)
        {
            this.position = position;
            this.eulerAngles = eulerAngles;
            this.hash = hash;
            this.networkMovement = networkMovement;
        }

        public Vector3 position
        {
            get => new Vector3(posx, posy, posz);
            set
            {
                posx = value.x;
                posy = value.y;
                posz = value.z;
            }
        }

        public Vector3 eulerAngles
        {
            get => new Vector3(rotx, roty, rotz);
            set
            {
                rotx = value.x;
                roty = value.y;
                rotz = value.z;
            }
        }
    }
}
