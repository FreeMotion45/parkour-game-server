using Assets.Scripts.Messages.ClientOrigin;
using Assets.Scripts.Server;
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
    class MoveTransformHandler : BaseBehaviourDatagramHandler
    {
        public override void Handle(DatagramHolder deserializedDatagram, NetworkChannel networkChannel)
        {
            MoveTransformMessage movePlayerMessage = (MoveTransformMessage)deserializedDatagram.Data;

            if (!Ownership.Owns(networkChannel, movePlayerMessage.transformHash)) return;

            GameObject player = NetTransform.networkObjects[movePlayerMessage.transformHash].gameObject;
            player.transform.position = movePlayerMessage.Position;
            player.transform.rotation = movePlayerMessage.Rotation;
        }
    }
}
