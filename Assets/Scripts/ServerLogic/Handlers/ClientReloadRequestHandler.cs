using Assets.Scripts.Network.Messages.ServerOrigin.Weapon;
using Assets.Scripts.ServerLogic.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityMultiplayer.Shared.Networking;
using UnityMultiplayer.Shared.Networking.Datagrams;
using UnityMultiplayer.Shared.Networking.Datagrams.Handling;

namespace Assets.Scripts.ServerLogic.Handlers
{
    class ClientReloadRequestHandler : BaseBehaviourDatagramHandler
    {
        public override void Handle(DatagramHolder deserializedDatagram, NetworkChannel networkChannel)
        {
            // Datagram is null. Checking the gun in hand.
            BaseGun playerGun = GetGunCurrentGun(networkChannel);
            bool reloadSuccessful = playerGun.TryReload();

            Debug.Log($"{networkChannel.ChannelID} successfully reloaded? " + reloadSuccessful);

            networkChannel.Send(new ServerReloadResponseMessage(reloadSuccessful),
                DatagramType.ServerReloadResponse);
        }

        private BaseGun GetGunCurrentGun(NetworkChannel channel)
        {
            return GamePlayers.GetComponent<Transform>(channel)
                .Find("Camera")
                .GetComponentInChildren<BaseGun>();
        }
    }
}
