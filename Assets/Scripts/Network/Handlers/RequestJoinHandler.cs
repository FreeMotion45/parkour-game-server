using Assets.Scripts.Messages;
using Assets.Scripts.Messages.ClientOrigin;
using Assets.Scripts.Messages.ServerOrigin;
using Assets.Scripts.ServerLogic;
using Assets.Scripts.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        public PlayerRespawner respawner;

        public override void Handle(DatagramHolder deserializedDatagram, NetworkChannel networkChannel)
        {
            RequestJoinMessage request = (RequestJoinMessage)deserializedDatagram.Data;

            Vector3 spawnPosition = respawner.GetRespawnPosition();
            GameObject player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
            player.name = request.name;

            GamePlayers.players[networkChannel] = player;

            LinkCurrentPlayersToIDs(networkChannel);
            Thread.Sleep(100);
            SendCurrentWorldState(networkChannel);
            Thread.Sleep(100);

            Debug.Log("Spawning player at: " + spawnPosition);

            PlayerJoinMessage playerJoinMessage = new PlayerJoinMessage(networkChannel.ChannelID, request.name, spawnPosition);
            GamePlayers.Publish(playerJoinMessage, DatagramType.PlayerJoin);
        }

        private void LinkCurrentPlayersToIDs(BaseNetworkChannel channel)
        {
            foreach (BaseNetworkChannel connectedChannel in GamePlayers.players.Keys)
            {
                string name = GamePlayers.GetName(connectedChannel);
                LinkPlayerNameToIDMessage link = new LinkPlayerNameToIDMessage(connectedChannel.ChannelID, name);
                channel.Send(link, DatagramType.LinkNameToID);
            }
        }

        private void SendCurrentWorldState(BaseNetworkChannel channel)
        {
            IEnumerable<PlayerInformation> playerInformation = GamePlayers.players.Keys
                .Where(network => network != channel)
                .Select(network =>
                {
                    Vector3 position = GamePlayers.GetPosition(network);
                    Quaternion rotation = GamePlayers.GetTransform(network).transform.Find("Camera").rotation;
                    return new PlayerInformation(network.ChannelID, position, rotation);
                });

            OnJoinWorldState world = new OnJoinWorldState(playerInformation);
            channel.Send(world, DatagramType.WorldState);
        }
    }
}
