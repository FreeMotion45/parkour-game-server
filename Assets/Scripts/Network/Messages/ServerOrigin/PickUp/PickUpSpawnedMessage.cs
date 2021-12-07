using Assets.Scripts.Game.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Network.Messages.ServerOrigin.PickUp
{
    [Serializable]
    class PickUpSpawnedMessage
    {
        public int pickUpId;
        public PickUpType pickUpType;
        public Vector3 position;

        public PickUpSpawnedMessage(int pickUpId, PickUpType pickUpType, Vector3 position)
        {
            this.pickUpId = pickUpId;
            this.pickUpType = pickUpType;
            this.position = position;
        }
    }
}
