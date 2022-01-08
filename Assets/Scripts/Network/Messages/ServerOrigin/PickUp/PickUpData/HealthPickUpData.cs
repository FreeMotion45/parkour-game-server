using Assets.Scripts.Game.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Network.Messages.ServerOrigin.PickUp.PickUpData
{
    [Serializable]
    class HealthPickUpData : BasePickUpData
    {
        public int healAmount;

        public HealthPickUpData(int healAmount) : base(PickUpType.Health)
        {
            this.healAmount = healAmount;
        }
    }
}
