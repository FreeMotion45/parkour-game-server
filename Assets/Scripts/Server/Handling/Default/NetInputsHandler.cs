using Assets.Scripts.Shared.Datagrams.Handling;
using Assets.Scripts.Shared.Datagrams.Messages;
using Assets.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityMultiplayer.Shared.Networking;
using UnityMultiplayer.Shared.Networking.Datagrams;

namespace Assets.Scripts.Server.Handling.Default
{
    class NetInputsHandler : IDatagramHandler
    {
        private readonly Dictionary<int, ServerNetInputs> netInputs = new Dictionary<int, ServerNetInputs>();
        private readonly BitReader bitReader = new BitReader();

        public void Handle(DatagramHolder deserializedDatagram, NetworkChannel networkChannel)
        {
            NetInputMessage netBytesMessage = (NetInputMessage)deserializedDatagram.Data;
            
            NetTransform netTransform = NetTransform.networkTransforms[netBytesMessage.transformHash];
            if (!netInputs.ContainsKey(netTransform.hash))
                netInputs.Add(netTransform.hash, netTransform.GetComponent<ServerNetInputs>());

            ServerNetInputs serverNetInputs = netInputs[netTransform.hash];
            serverNetInputs.SimulateByInput(netBytesMessage);
        }
    }
}
