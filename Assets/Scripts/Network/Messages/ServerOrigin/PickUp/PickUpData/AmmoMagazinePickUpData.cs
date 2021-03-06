using Assets.Scripts.Game.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Network.Messages.ServerOrigin.PickUp.PickUpData
{
    [Serializable]
    class AmmoMagazinePickUpData : BasePickUpData
    {
        public int magazinesRecovered;

        public AmmoMagazinePickUpData(int magazinesRecovered) : base(PickUpType.AmmoMagazine)
        {
            this.magazinesRecovered = magazinesRecovered;
        }
    }
}
