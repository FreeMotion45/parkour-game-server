using Assets.Scripts.Game.Shared;
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
        public LayerMask hittableLayers;

        private LayerMask temporaryLayer;

        void Start()
        {
            temporaryLayer = LayerMask.NameToLayer(PLAYER_SHOOT_IGNORE_LAYER);
        }

        public override void Handle(DatagramHolder deserializedDatagram, NetworkChannel networkChannel)
        {
            GamePlayers.Publish(deserializedDatagram, sender: networkChannel);

            // Get initial data.
            PlayerShootMessage message = (PlayerShootMessage)deserializedDatagram.Data;            

            // Moving the player to a temporary isolated layer,
            // so he doesn't hit himself when shooting.
            GameObject player = GamePlayers.GetPlayerObject(networkChannel);
            int originalLayer = player.layer;
            player.layer = temporaryLayer;

            ServerGun playerGun = GetGunCurrentGun(networkChannel);
            Dictionary<GameObject, ProjectileHitInfo> damageDealt = playerGun.CheckPlayersHit();

            // Moving the from the temporary layer to his original one.
            player.layer = originalLayer;

            foreach (GameObject objectHit in damageDealt.Keys)
            {
                if (objectHit.CompareTag("Player"))
                {
                    HandlePlayerHit(networkChannel, damageDealt[objectHit]);
                }
                else
                {
                    Debug.Log($"{GamePlayers.GetName(networkChannel)} shot and hit nothing");
                }
            }
        }

        private ServerGun GetGunCurrentGun(BaseNetworkChannel channel)
        {
            return GamePlayers.GetComponent<Transform>(channel)
                .Find("Camera")
                .GetComponentInChildren<ServerGun>();
        }

        private void HandlePlayerHit(NetworkChannel shooterChannel, ProjectileHitInfo hitInfo)
        {
            GameObject objectHit = hitInfo.hits[0].collider.gameObject;
            PlayerGameState playerState = objectHit.GetComponent<PlayerGameState>();
            BaseNetworkChannel hitChannel = GamePlayers.GetChannel(objectHit);
            int idOfHitPlayer = hitChannel.ChannelID;

            if (playerState.IsDead())
            {
                Debug.Log($"{GamePlayers.GetName(shooterChannel)} shot and hit a dead player");
                return;
            }
            
            int hitPlayerHealth = playerState.Damage(hitInfo.damageDealt);

            object data;
            DatagramType type;

            if (playerState.IsDead())
            {
                data = new PlayerKillMessage(idOfHitPlayer, shooterChannel.ChannelID);
                type = DatagramType.PlayerKill;
                Debug.Log($"{GamePlayers.GetName(shooterChannel)} killed {objectHit.name}");
                playerRespawner.RespawnPlayer(hitChannel);
            }
            else
            {
                Vector3[] relativeHitPositions = hitInfo.hits
                    .Select(bulletHit => bulletHit.point - objectHit.transform.position).ToArray();                
                data = new PlayerHitMessage(idOfHitPlayer, shooterChannel.ChannelID, hitPlayerHealth, relativeHitPositions);
                type = DatagramType.PlayerHit;
                Debug.Log($"{GamePlayers.GetName(shooterChannel)} reduced {objectHit.name}'s health to {playerState.health}");
            }

            GamePlayers.Publish(data, type);
        }
    }
}
