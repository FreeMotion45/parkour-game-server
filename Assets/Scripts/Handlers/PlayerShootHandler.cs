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
        public LayerMask hittableLayers;

        public override void Handle(DatagramHolder deserializedDatagram, NetworkChannel networkChannel)
        {
            // Get initial data.
            PlayerShootMessage message = (PlayerShootMessage)deserializedDatagram.Data;
            Quaternion playerRotation = message.Rotation;

            Transform playerTransform = PlayerDatabase.GetTransform(networkChannel);

            // Set player data.
            playerTransform.rotation = playerRotation;

            // Calculate shooting direction and origin.
            Vector3 direction = playerTransform.forward;
            Vector3 origin = playerTransform.GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));

            // Check if client hit another client.
            bool didHitAnyone = Physics.Raycast(origin, direction, out RaycastHit info, 300, hittableLayers);

            // If yes print a debug message.
            if (didHitAnyone)
            {                
                if (info.collider.CompareTag("Player"))
                {
                    Debug.Log($"{PlayerDatabase.GetName(networkChannel)} has hit {info.collider.name}");
                }
            }
        }
    }
}
