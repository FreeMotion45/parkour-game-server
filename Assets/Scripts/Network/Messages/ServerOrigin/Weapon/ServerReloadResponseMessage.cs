using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Network.Messages.ServerOrigin.Weapon
{
    [Serializable]
    class ServerReloadResponseMessage
    {
        public bool reloadSuccessful;

        public ServerReloadResponseMessage(bool reloadSuccessful)
        {
            this.reloadSuccessful = reloadSuccessful;
        }
    }
}
