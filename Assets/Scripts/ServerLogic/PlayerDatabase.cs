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
    class PlayerDatabase : MonoBehaviour
    {
        public static readonly Dictionary<BaseNetworkChannel, GameObject> players = new Dictionary<BaseNetworkChannel, GameObject>();

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

        public void OnDisconnect(BaseNetworkChannel disconnectedChannel)
        {
            if (players.ContainsKey(disconnectedChannel))
                players.Remove(disconnectedChannel);
        }
    }
}
