using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Messages.ServerOrigin
{
    [Serializable]
    class PlayersUpdateMessage
    {
        public PlayerInformation[] playerInformation;

        public PlayersUpdateMessage(IEnumerable<PlayerInformation> playerInformation)
        {
            this.playerInformation = playerInformation.ToArray();
        }
    }
}
