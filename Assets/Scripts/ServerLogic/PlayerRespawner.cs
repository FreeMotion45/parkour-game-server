using Assets.Scripts.Game.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.ServerLogic
{
    class PlayerRespawner : MonoBehaviour
    {
        [SerializeField] Transform[] spawnPositions;
        public float respawnTime;

        private int currentSpawnIndex;

        public void RespawnPlayer(GameObject player)
        {
            StartCoroutine(RespawnPlayerAfterTime(player));
        }

        private IEnumerator RespawnPlayerAfterTime(GameObject player)
        {
            yield return new WaitForSeconds(respawnTime);
            var state = player.GetComponent<PlayerGameState>();
            state.RevivePlayer(state.maxHealth);
        }

        public Vector3 GetRespawnPosition()
        {
            int spawnIndex = currentSpawnIndex % spawnPositions.Length;
            currentSpawnIndex++;

            return spawnPositions[spawnIndex].position;
        }
    }
}
