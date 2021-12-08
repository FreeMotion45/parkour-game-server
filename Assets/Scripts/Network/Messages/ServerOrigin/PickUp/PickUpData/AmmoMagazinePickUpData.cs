using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Network.Messages.ServerOrigin.PickUp.PickUpData
{
    [Serializable]
    class AmmoMagazinePickUpData
    {
        public int magazinesRecovered;

        public AmmoMagazinePickUpData(int magazinesRecovered)
        {
            this.magazinesRecovered = magazinesRecovered;
        }
    }
}
