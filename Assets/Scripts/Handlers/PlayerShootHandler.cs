using Assets.Scripts.Messages.ClientOrigin;
using Assets.Scripts.ServerLogic;
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
        public override void Handle(DatagramHolder deserializedDatagram, NetworkChannel networkChannel)
        {
            // Check if client hit another client
            PlayerShoot originPlayer = (PlayerShoot)deserializedDatagram.Data;
            Quaternion playerRotation = originPlayer.Rotation;

            Transform playerTransform = PlayerDatabase.GetNetTransform(networkChannel).transform;

            // set player
            playerTransform.rotation = playerRotation;

            // bullet data
            Vector3 direction = playerTransform.forward;
            Vector3 origin = playerTransform.GetComponent<Camera>().WorldToScreenPoint(new Vector3(0.5f, 0.5f));

            // check if client hit other client
            bool didHit = Physics.Raycast(origin, direction);

            // If yes print HIT in debug console.
            if (didHit) {
                Debug.Log($"{PlayerDatabase.GetName(networkChannel)} has hit someone");
            }
        }
    }
}
