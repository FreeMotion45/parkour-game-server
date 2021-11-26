using Assets.Scripts.Server.Behaviours;
using Assets.Scripts.Shared.Datagrams.Handling;
using Assets.Scripts.Shared.Datagrams.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityMultiplayer.Shared.Networking;
using UnityMultiplayer.Shared.Networking.Datagrams;

namespace Assets.Scripts.Server.Handling.Default
{
    class NetAbsoluteTransformHandler : IDatagramHandler
    {
        private readonly Dictionary<int, ServerNetInputs> netInputs = new Dictionary<int, ServerNetInputs>();
        public void Handle(DatagramHolder deserializedDatagram, NetworkChannel networkChannel)

        {
            NetAbsoluteTransform netAbsoluteTransform = (NetAbsoluteTransform)deserializedDatagram.Data;
            ServerNetTransform netTransform = ServerNetTransform.networkTransforms[netAbsoluteTransform.transformHash];
            
            if (!netInputs.ContainsKey(netTransform.hash))
                netInputs.Add(netTransform.hash, netTransform.GetComponent<ServerNetInputs>());

            ServerNetInputs inputs = netInputs[netTransform.hash];

            if (!inputs.clientControlsPosition || !inputs.clientControlsRotation) return;

            netTransform.transform.position = netAbsoluteTransform.position.Get();
            netTransform.transform.eulerAngles = netAbsoluteTransform.eulerAngles.Get();
        }
    }
}
