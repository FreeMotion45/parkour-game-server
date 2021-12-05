﻿using Assets.Scripts.Game.Shared;
using Assets.Scripts.Messages.ClientOrigin;
using Assets.Scripts.Network.Messages.ServerOrigin;
using Assets.Scripts.Network.Messages.ServerOrigin.PlayerState;
using Assets.Scripts.ServerLogic;
using Assets.Scripts.ServerLogic.Player;
using Assets.Scripts.Shared;
using System;
using System.Collections;
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

        public PlayerRespawner playerRespawner;
        public int defaultGunDamage = 20;

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


            // Moving the player to a temporary isolated layer,
            // so he doesn't hit himself when shooting.
            GameObject player = PlayerDatabase.GetPlayerObject(networkChannel);
            int originalLayer = player.layer;
            player.layer = temporaryLayer;

            Gun playerGun = PlayerDatabase.GetComponent<Gun>(networkChannel);            
            GameObject objectHit = playerGun.Shoot(playerRotation, hittableLayers);

            // Moving the from the temporary layer to his original one.
            player.layer = originalLayer;

            if (objectHit != null)
            {                
                HandleObjectHit(networkChannel, objectHit);
            }
            else
            {
                Debug.Log($"{PlayerDatabase.GetName(networkChannel)} shot and hit nothing");
            }
        }

        private void HandleObjectHit(NetworkChannel shooterChannel, GameObject objectHit)
        {
            if (!objectHit.CompareTag("Player")) return;

            PlayerGameState info = objectHit.GetComponent<PlayerGameState>();
            BaseNetworkChannel hitChannel = PlayerDatabase.GetChannel(objectHit);
            int idOfHitPlayer = hitChannel.ChannelID;

            if (info.IsDead())
            {
                Debug.Log($"{PlayerDatabase.GetName(shooterChannel)} shot and hit a dead player");
                return;
            }

            // TODO: Decouple this somehow.
            int hitPlayerHealth = info.Damage(defaultGunDamage);

            object data;
            DatagramType type;

            if (info.IsDead())
            {
                data = new PlayerKillMessage(idOfHitPlayer, shooterChannel.ChannelID);                
                type = DatagramType.PlayerKill;
                Debug.Log($"{PlayerDatabase.GetName(shooterChannel)} killed {objectHit.name}");
                playerRespawner.RespawnPlayer(hitChannel);
            }
            else
            {                
                data = new PlayerHitMessage(idOfHitPlayer, shooterChannel.ChannelID, hitPlayerHealth);
                type = DatagramType.PlayerHit;
                Debug.Log($"{PlayerDatabase.GetName(shooterChannel)} reduced {objectHit.name}'s health to {info.health}");
            }

            PlayerDatabase.Publish(data, type);
        }
    }
}
