using Assets.Scripts.Messages;
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
    class RequestJoinHandler : BaseBehaviourDatagramHandler
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Vector3 spawnPosition;

        public override void Handle(DatagramHolder deserializedDatagram, NetworkChannel networkChannel)
        {
            RequestJoinMessage request = (RequestJoinMessage)deserializedDatagram.Data;

            GameObject player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
            player.name = request.name;

            int transformHash = NetTransform.RegisterNewNetObject(player);
            Ownership.AddOwned(networkChannel, transformHash);

            IEnumerable<PlayerInformation> playerInformation = PlayerDatabase.players.Keys
                .Select(net => new PlayerInformation(
                    PlayerDatabase.GetName(net),
                    net.ChannelID,
                    PlayerDatabase.GetNetTransform(net).transformHash,
                    new Utils.SerializableVector3(PlayerDatabase.GetPosition(net)),
                    new Utils.SerializableVector3(PlayerDatabase.GetRotation(net))
                    ));
            WorldStateMessage world = new WorldStateMessage(playerInformation);
            networkChannel.Send(world, DatagramType.WorldState);

            PlayerDatabase.players[networkChannel] = player;

            PlayerJoinMessage playerJoinMessage = new PlayerJoinMessage(request.name, networkChannel.ChannelID, transformHash, spawnPosition);
            PlayerDatabase.Publish(playerJoinMessage, DatagramType.PlayerJoin);
        }
    }
}
