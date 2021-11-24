using Assets.Scripts.Shared.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityMultiplayer.Server;
using UnityMultiplayer.Shared.Networking.Datagrams;

namespace Assets.Scripts.Shared
{
    class GigaNetGlobals
    {
        public static PacketManager packetManager;
        public static InterpolationManager interpolationManager;

        public static void PublishMessage(DatagramHolder datagramHolder,
            IEnumerable<INetworkChannel> receivers = null,
            TransportType transportType = TransportType.Reliable)
        {
            if (receivers == null)
                receivers = packetManager.NetworkChannels;

            foreach (INetworkChannel channel in receivers)
            {
                channel.Send(datagramHolder, transportType);
            }
        }

        public static void PublishMessage(object data, DatagramType datagramType,
            IEnumerable<INetworkChannel> receivers = null,
            TransportType transportType = TransportType.Reliable)
        {
            DatagramHolder datagramHolder = new DatagramHolder(datagramType, data);
            PublishMessage(datagramHolder, receivers, transportType);
        }
    }
}
