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

namespace UnityMultiplayer.Server
{
    class PacketManager : MonoBehaviour
    {
        [SerializeField] private BaseGameObjectSerializer _serializer;
        [SerializeField] private DatagramHandlerResolver _datagramHandlerResolver;
        [SerializeField] private string _hostIP;
        [SerializeField] private int _hostPort;

        private List<BaseNetworkChannel> _networkChannels;
        private Dictionary<IPEndPoint, NetworkChannel> _hostToChannel;        
        private IPEndPoint _localEndPoint;
        private ReliableNetworkListener _reliableNetworkListener;        
        private UnreliableNetworkListener _unreliableNetworkListener;

        private int clientId;

        public IReadOnlyList<BaseNetworkChannel> NetworkChannels => _networkChannels;        

        public void Start()
        {            
            _networkChannels = new List<BaseNetworkChannel>();
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

                _networkChannels.Add(networkChannel);
                _hostToChannel[remote] = networkChannel;
                newChannels.Add(networkChannel);

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

            foreach (NetworkChannel networkChannel in _networkChannels)
            {
                DatagramHolder[] allMessages = networkChannel.GetAllReliableAndUnreliableMessages();
                if (!networkChannel.IsConnected)
                {
                    // TODO: Uncomment these and implement them in a normal manner.
                    // ClientDataHolder.RemoveClient(networkChannel.RemoteEndPoint);
                    //_hostToChannel.Remove(networkChannel.RemoteEndPoint);
                    continue;
                }
                foreach (DatagramHolder message in allMessages)
                {
                    ProcessMessage(message, networkChannel);
                }
            }
        }

        private void ProcessMessage(DatagramHolder datagramHolder, NetworkChannel sender)
        {                        
            DatagramType datagramType = datagramHolder.DatagramType;
            _datagramHandlerResolver.Resolve(datagramType).Handle(datagramHolder, sender);
        }
    }
}
