using UnityMultiplayer.Shared.Networking.Datagrams;
using UnityMultiplayer.Shared.Networking.Serializers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using UnityEngine;

namespace UnityMultiplayer.Shared.Networking.SecureConnection
{
    public class ReliableNetworkClient : IDisposable
    {
        private bool _disposed;

        private BufferBlock<DatagramHolder> _sendQueue;
        private CancellationTokenSource _sendCancellationToken;
        private Task _sendLoopTask;

        private readonly IPEndPoint _remoteEndPoint;
        private readonly ReliableNetworkMessager _messager;
        private readonly BaseGameObjectSerializer _serializer;

        public ReliableNetworkClient(IPEndPoint remoteEndPoint,
            ReliableNetworkMessager messageReader,
            BaseGameObjectSerializer serializer)
        {
            Client = new TcpClient();
            _messager = messageReader;
            _serializer = serializer;
            _remoteEndPoint = remoteEndPoint;
        }

        public ReliableNetworkClient(TcpClient client,
            ReliableNetworkMessager messager,
            BaseGameObjectSerializer serializer)
        {
            Client = client;
            _messager = messager;
            _serializer = serializer;
        }

        public TcpClient Client { get; }
        public BaseGameObjectSerializer Serializer => _serializer;
        public bool RemoteOpen { get; private set; }
        public IPEndPoint RemoteEndPoint => _remoteEndPoint;
        public IPEndPoint LocalEndPoint => (IPEndPoint)Client.Client.LocalEndPoint;
        public bool IsConnected => Client.Connected;

        public void Connect()
        {
            if (!Client.Connected)
            {
                Client.Connect(_remoteEndPoint);
            }

            _sendCancellationToken = new CancellationTokenSource();
            _sendQueue = new BufferBlock<DatagramHolder>();
            _sendLoopTask = Task.Run(SendLoopAsync);
        }

        public void Disconnect()
        {
            if (_disposed) return;
            _disposed = true;

            _sendCancellationToken.Cancel();
            Client.Close();
            _sendLoopTask.Wait();
            Client.Dispose();
        }

        public void Dispose()
        {
            Disconnect();
        }

        public void SendDatagramHolder(DatagramHolder datagramHolder)
        {
            if (!IsConnected) return;
            _messager.WriteMessage(Client, _serializer.Serialize(datagramHolder));
        }

        public void AsyncSendDatagramHolder(DatagramHolder datagramHolder)
        {
            if (!IsConnected) return;
            _sendQueue.Post(datagramHolder);
        }

        public DatagramHolder[] ReadAvailableMessages()
        {
            return _messager.ReadAvailableMessages(Client)
                .Select(messageBytes => _serializer.Deserialize(messageBytes))
                .ToArray();
        }

        private async Task SendLoopAsync()
        {
            while (IsConnected)
            {
                try
                {
                    DatagramHolder dgram = await _sendQueue.ReceiveAsync(_sendCancellationToken.Token);
                    _messager.WriteMessage(Client, _serializer.Serialize(dgram));
                }
                catch (OperationCanceledException)
                {
                    // This means we disconnect the client. Its OK.
                    break;
                }
                catch (Exception exc)
                {
                    // This means that we can't send a message because the client is closed.
                    // Its also OK.
                    RemoteOpen = false;
                    break;
                }
            }
            Debug.Log("SendLoop done.");
        }
    }
}
