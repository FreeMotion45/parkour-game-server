using Assets.Scripts.Messages.ClientOrigin;
using Assets.Scripts.ServerLogic;
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
        public override void Handle(DatagramHolder deserializedDatagram, NetworkChannel networkChannel)
        {
            MovePlayerMessage movePlayerMessage = (MovePlayerMessage)deserializedDatagram.Data;
            GameObject player = PlayerDatabase.players[networkChannel];
            player.transform.position = movePlayerMessage.Position;
            player.transform.eulerAngles = movePlayerMessage.EulerAngles;
        }
    }
}
