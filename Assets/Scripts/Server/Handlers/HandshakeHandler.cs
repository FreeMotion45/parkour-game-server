using Assets.Scripts.Shared.Datagrams.Handling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityMultiplayer.Shared.Networking;
using UnityMultiplayer.Shared.Networking.Datagrams;

namespace Assets.Scripts.Server.Handlers
{
    class HandshakeHandler : IDatagramHandler
    {
        public void Handle(DatagramHolder deserializedDatagram, NetworkChannel networkChannel)
        {
            networkChannel.Send(null, DatagramType.Handshake);
            var remote = networkChannel.RemoteEndPoint;
            Debug.Log($"Successfully performed hand shake with: {remote.Address}:{remote.Port}");
        }
    }
}
