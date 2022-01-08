using Assets.Scripts.Game.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Network.Messages.ServerOrigin.PickUp.PickUpData
{
    [Serializable]
    class BasePickUpData
    {
        public readonly PickUpType pickUpType;

        public BasePickUpData(PickUpType pickUpType)
        {
            this.pickUpType = pickUpType;
        }
    }
}
