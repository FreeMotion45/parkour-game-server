using Assets.Scripts.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityMultiplayer.Shared.Networking;
using UnityMultiplayer.Shared.Networking.Datagrams;

namespace Assets.Scripts.ServerLogic
{
    class PlayerDatabase
    {
        public static readonly Dictionary<NetworkChannel, GameObject> players = new Dictionary<NetworkChannel, GameObject>();

        public static void Publish(object data,
            DatagramType datagramType,
            NetworkChannel sender = null,
            TransportType transport = TransportType.Reliable)
        {
            foreach (NetworkChannel channel in players.Keys)
            {
                if (sender == channel) continue;
                channel.Send(data, datagramType, transport);
            }
        }
    }
}
