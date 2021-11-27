using Assets.Scripts.Messages;
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
    class PeriodicalTransformDataSender : MonoBehaviour
    {
        public static PeriodicalTransformDataSender Instance;

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

                TransformsUpdateMessage playerTransformsMessage = new TransformsUpdateMessage(
                    netTransforms.Select(nt => nt.transformHash),
                    netTransforms.Select(nt => nt.transform.position),
                    netTransforms.Select(nt => nt.transform.rotation)
                    );                
                PlayerDatabase.Publish(playerTransformsMessage, DatagramType.TransformsUpdate);                
            }            
        }

        public void Add(NetTransform netTransform)
        {
            netTransforms.Add(netTransform);
        }
    }
}
