using Assets.Scripts.Messages;
using Assets.Scripts.Messages.ClientOrigin;
using Assets.Scripts.Messages.ServerOrigin;
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
        public PlayerRespawner respawner;

        public override void Handle(DatagramHolder deserializedDatagram, NetworkChannel networkChannel)
        {
            RequestJoinMessage request = (RequestJoinMessage)deserializedDatagram.Data;

            Vector3 spawnPosition = respawner.GetRespawnPosition();
            GameObject player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
            player.name = request.name;

            PlayerDatabase.players[networkChannel] = player;

            LinkCurrentPlayersToIDs(networkChannel);
            SendCurrentWorldState(networkChannel);            

            PlayerJoinMessage playerJoinMessage = new PlayerJoinMessage(request.name, networkChannel.ChannelID, spawnPosition);
            PlayerDatabase.Publish(playerJoinMessage, DatagramType.PlayerJoin);
        }

        private void LinkCurrentPlayersToIDs(BaseNetworkChannel channel)
        {
            foreach (BaseNetworkChannel connectedChannel in PlayerDatabase.players.Keys)
            {
                string name = PlayerDatabase.GetName(connectedChannel);
                LinkPlayerNameToIDMessage link = new LinkPlayerNameToIDMessage(name, connectedChannel.ChannelID);
                channel.Send(link, DatagramType.LinkNameToID);
            }
        }

        private void SendCurrentWorldState(BaseNetworkChannel channel)
        {
            IEnumerable<PlayerInformation> playerInformation = PlayerDatabase.players.Keys
                .Where(network => network != channel)
                .Select(network =>
                {
                    Vector3 position = PlayerDatabase.GetPosition(network);
                    Quaternion rotation = PlayerDatabase.GetTransform(network).transform.Find("Camera").rotation;
                    return new PlayerInformation(network.ChannelID, position, rotation);
                });

            OnJoinWorldState world = new OnJoinWorldState(playerInformation);
            channel.Send(world, DatagramType.WorldState);
        }
    }
}
