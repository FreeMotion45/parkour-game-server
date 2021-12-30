using Assets.Scripts.Messages;
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

        private readonly List<GameObject> netTransforms = new List<GameObject>();

        public float interpolationTime = 0.2f;

        void Start()
        {
            StartCoroutine(SendTransformData());
            Instance = this;
        }        

        IEnumerator SendTransformData()
        {
            while (isActiveAndEnabled)
            {
                yield return new WaitForSeconds(interpolationTime);
                SendPlayersUpdate();
                SendTransformsUpdate();
            }            
        }

        private void SendPlayersUpdate()
        {
            if (GamePlayers.players.Count == 0) return;

            IEnumerable<PlayerInformation> playerInformation = GamePlayers.players
                .Select(pair =>
                {
                    Transform transform = pair.Value.transform;
                    Quaternion rotation = pair.Value.transform.Find("Camera").rotation;
                    return new PlayerInformation(pair.Key.ChannelID, transform.position, rotation);
                });

            PlayersUpdateMessage playersUpdateMessage = new PlayersUpdateMessage(playerInformation);
            GamePlayers.Publish(playersUpdateMessage, DatagramType.PlayersUpdate, transport: Shared.TransportType.Unreliable);
        }

        private void SendTransformsUpdate()
        {
            if (netTransforms.Count == 0) return;

            IEnumerable<Vector3> positions = netTransforms.Select(net => net.transform.position);
            IEnumerable<Quaternion> rotations = netTransforms.Select(net => net.transform.rotation);
            IEnumerable<int> hashes = netTransforms.Select(net => NetTransform.objectHash[net]);

            TransformsUpdateMessage transformsUpdateMessage = new TransformsUpdateMessage(hashes, positions, rotations);
            GamePlayers.Publish(transformsUpdateMessage, DatagramType.TransformsUpdate, transport: Shared.TransportType.Unreliable);
        }

        public void Add(GameObject netTransform)
        {
            netTransforms.Add(netTransform);
        }

        public void Remove(GameObject netTransform)
        {
            netTransforms.Remove(netTransform);
        }
    }
}
