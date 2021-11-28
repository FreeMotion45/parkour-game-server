﻿using Assets.Scripts.Messages;
using Assets.Scripts.Messages.ServerOrigin;
using Assets.Scripts.ServerLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityMultiplayer.Shared.Networking;
using UnityMultiplayer.Shared.Networking.Datagrams;

namespace Assets.Scripts.Server
{
    class PeriodicalPlayerInformationSender : MonoBehaviour
    {
        public static PeriodicalPlayerInformationSender Instance;

        private readonly List<NetTransform> netTransforms = new List<NetTransform>();

        public float interpolationTime = 0.2f;

        private void Start()
        {
            StartCoroutine(SendTransformData());
            Instance = this;
        }

        IEnumerator SendTransformData()
        {
            while (isActiveAndEnabled)
            {
                yield return new WaitForSeconds(interpolationTime);

                if (PlayerDatabase.players.Count == 0) continue;

                IEnumerable<PlayerInformation> playerInformation = PlayerDatabase.players
                    .Select(pair =>
                    {
                        Transform transform = pair.Value.transform;
                        Quaternion rotation = pair.Value.transform.Find("Camera").rotation;
                        return new PlayerInformation(pair.Key.ChannelID, transform.position, rotation);
                    });
                PlayersUpdateMessage playersUpdateMessage = new PlayersUpdateMessage(playerInformation);

                PlayerDatabase.Publish(playersUpdateMessage, DatagramType.PlayersUpdate);                
            }            
        }

        public void Add(NetTransform netTransform)
        {
            netTransforms.Add(netTransform);
        }
    }
}