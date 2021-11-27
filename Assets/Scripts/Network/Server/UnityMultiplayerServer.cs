using UnityMultiplayer.Shared;
using UnityMultiplayer.Shared.Networking;
using UnityMultiplayer.Shared.Networking.Datagrams;
using UnityMultiplayer.Shared.Networking.SecureConnection;
using UnityMultiplayer.Shared.Networking.Serializers;
using UnityMultiplayer.Shared.Networking.UnreliableConnection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityMultiplayer.Shared.Networking.Datagrams.Handling;
using Assets.Scripts.Shared;
using UnityEngine.Events;

namespace UnityMultiplayer.Server
{
    [Serializable]
    public class OnDisconnectEvent : UnityEvent<BaseNetworkChannel>
    { }

    class UnityMultiplayerServer : MonoBehaviour
    {
        [SerializeField] private string _hostIP;
        [SerializeField] private int _hostPort;
        [Space]
        [SerializeField] private DatagramHandlerResolver _datagramHandlerResolver;        
        [Space]
        [SerializeField] private BaseGameObjectSerializer _serializer;
        [Space]
        public OnDisconnectEvent OnDisconnect;

        private List<BaseNetworkChannel> _networkChannels;
        private List<BaseNetworkChannel> _nonHandshakedChannels;
        private Dictionary<IPEndPoint, NetworkChannel> _hostToChannel; // Linking TCP connections to their UDP counter parts.

        private IPEndPoint _localEndPoint;
        private ReliableNetworkListener _reliableNetworkListener;        
        private UnreliableNetworkListener _unreliableNetworkListener;        

        private int clientId;

        public IReadOnlyList<BaseNetworkChannel> NetworkChannels => _networkChannels;        

        public void Start()
        {            
            _networkChannels = new List<BaseNetworkChannel>();
            _nonHandshakedChannels = new List<BaseNetworkChannel>();
            _hostToChannel = new Dictionary<IPEndPoint, NetworkChannel>();            
            _localEndPoint = new IPEndPoint(IPAddress.Parse(_hostIP), _hostPort);

            _reliableNetworkListener = new ReliableNetworkListener(_localEndPoint, new ReliableNetworkMessager(), _serializer);
            _unreliableNetworkListener = new UnreliableNetworkListener(_localEndPoint, _hostToChannel, _serializer);            

            _reliableNetworkListener.Start();
        }        

        public void OnDisable()
        {
            _reliableNetworkListener.CloseListener();
            _unreliableNetworkListener.CloseListener();
            foreach (NetworkChannel channel in _networkChannels)
            {
                channel.Dispose();
            }
            _networkChannels.Clear();
        }

        public List<NetworkChannel> CreateNewChannels()
        {
            ReliableNetworkClient[] newClients = _reliableNetworkListener.AcceptNewConnections();
            List<NetworkChannel> newChannels = new List<NetworkChannel>();
            foreach (ReliableNetworkClient client in newClients)
            {
                clientId++;
                client.Connect();

                // Configure the UnreliableNetworkClient to send through an existing UdpClient and read unreliable messages virtually.
                IPEndPoint remote = (IPEndPoint)client.Client.Client.RemoteEndPoint;
                UnreliableNetworkClient unreliableNetworkClient = new UnreliableNetworkClient(remote, _serializer, _unreliableNetworkListener.UdpClient);                
                unreliableNetworkClient.ThisInitiatedConnection = false;
                unreliableNetworkClient.Connect();

                NetworkChannel networkChannel = new NetworkChannel(client, unreliableNetworkClient, clientId);

                //_networkChannels.Add(networkChannel);
                _hostToChannel[remote] = networkChannel;
                _nonHandshakedChannels.Add(networkChannel);

                Debug.Log($"Incoming connection from {remote.Address}:{remote.Port}");
            }
            return newChannels;
        }

        public void FixedUpdate()
        {
            CreateNewChannels();
            // Unreliable connection is not really a connection.
            // We virtually insert datagrams into it by reading from the network into the channel.
            _unreliableNetworkListener.ReadIntoChannels();

            AcceptHandshakes();

            foreach (NetworkChannel networkChannel in _networkChannels.ToArray())
            {
                DatagramHolder[] allMessages = networkChannel.GetAllReliableAndUnreliableMessages();
                if (!networkChannel.IsConnected)
                {
                    DisconnectChannel(networkChannel);
                    Debug.Log("Client disconnected without disconnect request...");
                    continue;
                }                

                foreach (DatagramHolder message in allMessages)
                {                    
                    if (message.DatagramType == DatagramType.Disconnect)
                    {
                        var remote = networkChannel.RemoteEndPoint;
                        DisconnectChannel(networkChannel);                        
                        Debug.Log($"Client {remote.Address}:{remote.Port} disconnected gracefully from the server.");

                        // Breaking because after the disconnect we do no want
                        // to process any other messages sent by the client.
                        break;
                    }
                    
                    ProcessMessage(message, networkChannel);
                }
            }
        }

        private void AcceptHandshakes()
        {
            foreach (NetworkChannel nonHandshakedClient in _nonHandshakedChannels.ToArray())
            {
                foreach (DatagramHolder handshake in nonHandshakedClient.GetAllReliableAndUnreliableMessages())
                {
                    var remote = nonHandshakedClient.RemoteEndPoint;
                    if (handshake.DatagramType == DatagramType.Handshake)
                    {
                        nonHandshakedClient.ReliableChannel.ServerConfirmHandshake(nonHandshakedClient.ChannelID);
                        _nonHandshakedChannels.Remove(nonHandshakedClient);
                        _networkChannels.Add(nonHandshakedClient);
                        Debug.Log($"Successfully performed hand shake with: {remote.Address}:{remote.Port}");
                    }
                    else
                    {
                        DisconnectChannel(nonHandshakedClient);
                        Debug.LogWarning($"Client {remote.Address}:{remote.Port} did not send a handshake before sending data.");
                    }
                }
            }
        }

        private void DisconnectChannel(NetworkChannel networkChannel)
        {
            if (networkChannel.PerformedHandshake)
                OnDisconnect.Invoke(networkChannel);

            if (_networkChannels.Contains(networkChannel))
                _networkChannels.Remove(networkChannel);

            _hostToChannel.Remove(networkChannel.RemoteEndPoint);
            networkChannel.Dispose();                      
        }

        private void ProcessMessage(DatagramHolder datagramHolder, NetworkChannel sender)
        {                        
            DatagramType datagramType = datagramHolder.DatagramType;
            _datagramHandlerResolver.Resolve(datagramType).Handle(datagramHolder, sender);
        }
    }
}
