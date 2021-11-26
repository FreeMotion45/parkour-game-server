using Assets.Scripts.Messages;
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
    class RequestJoinHandler : BaseBehaviourDatagramHandler
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Vector3 spawnPosition;

        public override void Handle(DatagramHolder deserializedDatagram, NetworkChannel networkChannel)
        {
            RequestJoinMessage request = (RequestJoinMessage)deserializedDatagram.Data;

            GameObject player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
            player.name = request.name;

            PlayerDatabase.players[networkChannel] = player;

            PlayerJoinMessage playerJoinMessage = new PlayerJoinMessage(request.name, spawnPosition);
            PlayerDatabase.Publish(playerJoinMessage, DatagramType.PlayerJoin);
        }
    }
}
