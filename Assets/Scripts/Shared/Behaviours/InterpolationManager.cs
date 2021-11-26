using Assets.Scripts.Server;
using Assets.Scripts.Server.Behaviours;
using Assets.Scripts.Shared.Datagrams.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityMultiplayer.Shared.Networking.Datagrams;

namespace Assets.Scripts.Shared.Behaviours
{
    class InterpolationManager : MonoBehaviour
    {
        public float interpolationTime = 0.2f;

        private List<ServerNetTransform> interpolatedNetTransforms;
        private float nextTimeToSendMessage;

        private void Start()
        {
            interpolatedNetTransforms = new List<ServerNetTransform>();            
            nextTimeToSendMessage = Time.time + interpolationTime;
            GigaNetServerGlobals.interpolationManager = this;
        }

        private void Update()
        {
            if (nextTimeToSendMessage < Time.time)
            {
                PublishNetTransformUpdate();
                nextTimeToSendMessage += interpolationTime;
            }
        }

        public void Add(ServerNetTransform netTransform)
        {
            interpolatedNetTransforms.Add(netTransform);
        }

        public void Remove(ServerNetTransform netTransform)
        {
            if (interpolatedNetTransforms.Contains(netTransform))
            {
                interpolatedNetTransforms.Remove(netTransform);
            }
        }

        private void PublishNetTransformUpdate()
        {
            foreach (ServerNetTransform netTransform in interpolatedNetTransforms)
            {
                UpdateNetTransformMessage updateNetTransform = new UpdateNetTransformMessage(
                    netTransform.transform.position,
                    netTransform.transform.eulerAngles,
                    NetworkMovement.Interpolated, netTransform.hash);
                GigaNetServerGlobals.PublishMessage(updateNetTransform, DatagramType.Transform);
            }            
        }
    }
}
