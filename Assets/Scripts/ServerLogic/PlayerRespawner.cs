using Assets.Scripts.Game.Shared;
using Assets.Scripts.Network.Messages.ServerOrigin;
using Assets.Scripts.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityMultiplayer.Shared.Networking.Datagrams;

namespace Assets.Scripts.ServerLogic
{
    class PlayerRespawner : MonoBehaviour
    {
        [SerializeField] Transform[] spawnPositions;
        public float respawnTime;

        private int currentSpawnIndex;

        public void RespawnPlayer(BaseNetworkChannel channel)
        {
            StartCoroutine(RespawnPlayerAfterTime(channel));
        }

        private IEnumerator RespawnPlayerAfterTime(BaseNetworkChannel player)
        {
            yield return new WaitForSeconds(respawnTime);

            Vector3 spawnPosition = GetRespawnPosition();
            var state = GamePlayers.GetComponent<PlayerGameState>(player);
            state.RevivePlayer(state.maxHealth);

            // Not doing the line below because the client is position authorative.
            // player.transform.position = spawnPosition;

            PlayerSpawnMessage spawnMessage = new PlayerSpawnMessage(player.ChannelID, spawnPosition);
            GamePlayers.Publish(spawnMessage, DatagramType.PlayerSpawn);
        }

        public Vector3 GetRespawnPosition()
        {
            int spawnIndex = currentSpawnIndex % spawnPositions.Length;
            currentSpawnIndex++;

            return spawnPositions[spawnIndex].position;
        }        
    }
}
