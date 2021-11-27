using Assets.Scripts.Messages.ClientOrigin;
using Assets.Scripts.ServerLogic;
using Assets.Scripts.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityMultiplayer.Shared.Networking;
using UnityMultiplayer.Shared.Networking.Datagrams;
using UnityMultiplayer.Shared.Networking.Datagrams.Handling;

namespace Assets.Scripts.Handlers
{
    class MovePlayerHandler : BaseBehaviourDatagramHandler
    {
        private readonly Dictionary<BaseNetworkChannel, Camera> cameras = new Dictionary<BaseNetworkChannel, Camera>();

        public override void Handle(DatagramHolder deserializedDatagram, NetworkChannel networkChannel)
        {
            MovePlayerMessage movement = (MovePlayerMessage)deserializedDatagram.Data;            
            Transform playerTransform = PlayerDatabase.GetTransform(networkChannel);

            playerTransform.position = movement.Position;
            if (!cameras.ContainsKey(networkChannel))
                cameras[networkChannel] = playerTransform.Find("Camera").GetComponent<Camera>();

            cameras[networkChannel].transform.rotation = movement.Rotation;            
        }
    }
}
