using Assets.Scripts.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityMultiplayer.Shared.Networking.Datagrams;
using UnityMultiplayer.Shared.Networking.SecureConnection;

namespace UnityMultiplayer.Shared.Networking
{
    public class NetworkChannel : INetworkChannel
    {
        public NetworkChannel(ReliableNetworkClient reliableChannel, UnreliableNetworkClient unreliableChannel)
        {
            ReliableChannel = reliableChannel;
            UnreliableChannel = unreliableChannel;
        }

        public ReliableNetworkClient ReliableChannel { get; private set; }
        public UnreliableNetworkClient UnreliableChannel { get; private set; }
        public IPEndPoint RemoteEndPoint { get => (IPEndPoint)ReliableChannel.Client.Client.RemoteEndPoint; }
        public bool IsConnected { get => UnreliableChannel.IsConnected && ReliableChannel.IsConnected; }

        public void Send(DatagramHolder data, TransportType transportType = TransportType.Reliable)
        {
            if (transportType == TransportType.Reliable)
                ReliableChannel.AsyncSendDatagramHolder(data);

            else if (transportType == TransportType.Unreliable)
                UnreliableChannel.SendDatagramHolder(data);
        }

        public void Send(object data, DatagramType datagramType, TransportType transportType = TransportType.Reliable)
        {
            Send(new DatagramHolder(datagramType, data), transportType);
        }

        public DatagramHolder[] GetAllReliableAndUnreliableMessages()
        {
            DatagramHolder[] receivedReliables = ReliableChannel.ReadAvailableMessages();
            DatagramHolder[] receivedUnreliables = UnreliableChannel.ReceiveAllMessages();

            List<DatagramHolder> datagramHolders = receivedReliables.ToList();
            datagramHolders.AddRange(receivedUnreliables);
            return datagramHolders.ToArray();
        }

        public void Connect()
        {
            ReliableChannel.Connect();
            UnreliableChannel.Connect();
        }

        public void Close()
        {
            ReliableChannel.Disconnect();
            ReliableChannel = null;

            UnreliableChannel.Disconnect();
            UnreliableChannel = null;
        }
    }
}
