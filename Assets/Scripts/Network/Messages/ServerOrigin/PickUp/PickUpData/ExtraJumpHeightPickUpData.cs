using Assets.Scripts.Game.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Network.Messages.ServerOrigin.PickUp.PickUpData
{
    [Serializable]
    class ExtraJumpHeightPickUpData : BasePickUpData
    {
        public float extraJumpHeight;

        public ExtraJumpHeightPickUpData(float extraJumpHeight) : base(PickUpType.ExtraJumpHeight)
        {
            this.extraJumpHeight = extraJumpHeight;
        }
    }
}
