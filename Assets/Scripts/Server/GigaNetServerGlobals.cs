using Assets.Scripts.Shared;
using Assets.Scripts.Shared.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityMultiplayer.Server;
using UnityMultiplayer.Shared.Networking.Datagrams;

namespace Assets.Scripts.Server
{
    class GigaNetServerGlobals
    {
        public static PacketManager packetManager;
        public static InterpolationManager interpolationManager;
        public static PhysicsSimulator physics;

        public static void PublishMessage(DatagramHolder datagramHolder,
            IEnumerable<BaseNetworkChannel> receivers = null,
            TransportType transportType = TransportType.Reliable)
        {
            if (receivers == null)
                receivers = packetManager.NetworkChannels;

            foreach (BaseNetworkChannel channel in receivers)
            {
                channel.Send(datagramHolder, transportType);
            }
        }

        public static void PublishMessage(object data, DatagramType datagramType,
            IEnumerable<BaseNetworkChannel> receivers = null,
            TransportType transportType = TransportType.Reliable)
        {
            DatagramHolder datagramHolder = new DatagramHolder(datagramType, data);
            PublishMessage(datagramHolder, receivers, transportType);
        }
    }
}
