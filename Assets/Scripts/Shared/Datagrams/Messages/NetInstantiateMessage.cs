using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Shared.Datagrams.Messages
{
    [Serializable]
    class NetInstantiateMessage
    {
        public int hash;        
        public string prefabName;

        private float posx;
        private float posy;
        private float posz;

        public NetInstantiateMessage(string prefabName, Vector3 position, int hash)
        {
            this.position = position;
            this.hash = hash;
            this.prefabName = prefabName;
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
    }
}
