using Assets.Scripts.Game.Shared;
using Assets.Scripts.Messages.ClientOrigin;
using Assets.Scripts.Network.Messages.ServerOrigin;
using Assets.Scripts.ServerLogic;
using Assets.Scripts.ServerLogic.Player;
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
    class PlayerShootHandler : BaseBehaviourDatagramHandler
    {
        private readonly string PLAYER_SHOOT_IGNORE_LAYER = "Player Shoot Ignore";

        private LayerMask temporaryLayer;

        private void Start()
        {
            temporaryLayer = LayerMask.NameToLayer(PLAYER_SHOOT_IGNORE_LAYER);
        }

        public LayerMask hittableLayers;

        public override void Handle(DatagramHolder deserializedDatagram, NetworkChannel networkChannel)
        {
            PlayerDatabase.Publish(deserializedDatagram, sender: networkChannel);

            // Get initial data.
            PlayerShootMessage message = (PlayerShootMessage)deserializedDatagram.Data;
            Quaternion playerRotation = message.Rotation;

            GameObject player = PlayerDatabase.GetPlayerObject(networkChannel);
            int originalLayer = player.layer;
            player.layer = temporaryLayer;

            Gun playerGun = PlayerDatabase.GetComponent<Gun>(networkChannel);            
            GameObject objectHit = playerGun.Shoot(playerRotation, hittableLayers);

            player.layer = originalLayer;

            if (objectHit != null)
            {
                if (gameObject == PlayerDatabase.GetPlayerObject(networkChannel)) return;
                HandleObjectHit(networkChannel, objectHit);
            }
            else
            {
                Debug.Log($"{PlayerDatabase.GetName(networkChannel)} shot and hit nothing");
            }
        }

        private void HandleObjectHit(NetworkChannel channel, GameObject gameObject)
        {
            if (!gameObject.CompareTag("Player")) return;

            PlayerGameInformation info = gameObject.GetComponent<PlayerGameInformation>();

            if (info.IsDead())
            {
                Debug.Log($"{PlayerDatabase.GetName(channel)} shot and hit a dead player");
                return;
            }

            // TODO: Decouple this somehow.
            int currentHealth = info.Damage(15);

            object data;
            DatagramType type;

            if (info.IsDead())
            {
                data = new PlayerDeathMessage(channel.ChannelID);                
                type = DatagramType.PlayerDeath;
                Debug.Log($"{PlayerDatabase.GetName(channel)} killed {gameObject.name}");
            }
            else
            {
                data = new PlayerHealthChangeMessage(channel.ChannelID, currentHealth);
                type = DatagramType.PlayerHealthChange;
                Debug.Log($"{PlayerDatabase.GetName(channel)} reduce {gameObject.name}'s health to {info.health}");
            }

            PlayerDatabase.Publish(data, type);
        }
    }
}
