using Assets.Scripts.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityMultiplayer.Shared.Networking.Datagrams;
using UnityMultiplayer.Shared.Networking.SecureConnection;
using UnityMultiplayer.Shared.Networking.Serializers;

namespace UnityMultiplayer.Shared.Networking
{
    public class NetworkChannel : BaseNetworkChannel, IDisposable
    {
        private bool _disposed;
        private bool _shouldConnect;

        public NetworkChannel(ReliableNetworkClient reliableChannel, UnreliableNetworkClient unreliableChannel)
        {
            ReliableChannel = reliableChannel;
            UnreliableChannel = unreliableChannel;

            // We receive channels that are already opened.
            _shouldConnect = false;
        }

        public NetworkChannel(IPEndPoint remoteEndPoint, BaseGameObjectSerializer serializer)
        {
            // We need to create and connect the channels.
            ReliableChannel = new ReliableNetworkClient(remoteEndPoint, new ReliableNetworkMessager(), serializer);
            _shouldConnect = true;
        }

        public ReliableNetworkClient ReliableChannel { get; private set; }
        public UnreliableNetworkClient UnreliableChannel { get; private set; }
        public IPEndPoint RemoteEndPoint => ReliableChannel.RemoteEndPoint;
        public bool IsConnected => UnreliableChannel.IsConnected && ReliableChannel.IsConnected;

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

            if (_shouldConnect)
            {
                // We can only bind to the same local port after we know on which port the TCP connection is binded to.
                UnreliableChannel = new UnreliableNetworkClient(RemoteEndPoint, ReliableChannel.Serializer, ReliableChannel.LocalEndPoint);
                UnreliableChannel.ThisInitiatedConnection = true;
            }

            UnreliableChannel.Connect();
        }

        public void Disconnect()
        {
            if (_disposed) return;
            _disposed = true;

            ReliableChannel.Disconnect();
            ReliableChannel = null;

            UnreliableChannel.Disconnect();
            UnreliableChannel = null;
        }

        public void Dispose()
        {
            if (_disposed) return;
            Disconnect();
        }

        public override void Send(DatagramHolder data, TransportType transportType = TransportType.Reliable)
        {
            if (transportType == TransportType.Reliable)
                ReliableChannel.AsyncSendDatagramHolder(data);
            else
                UnreliableChannel.SendDatagramHolder(data);
        }
    }
}
