using Assets.Scripts.Shared.Datagrams.Messages;
using Assets.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityMultiplayer.Shared.Networking.Datagrams;

namespace Assets.Scripts.Server.Behaviours
{
    public class ServerNetTransform : MonoBehaviour
    {
        private static int currentHash;

        public static int NextHash()
        {
            return ++currentHash;
        }

        public static Dictionary<int, ServerNetTransform> networkTransforms = new Dictionary<int, ServerNetTransform>();

        public static GameObject NetInstantiate(string prefab, Vector3 position)
        {
            int hash = NextHash();

            GameObject instantiatedNetObject = GameObject.Instantiate(NetPrefabs.netPrefabs[prefab], position, Quaternion.identity);
            ServerNetTransform netTransform = instantiatedNetObject.GetComponent<ServerNetTransform>();
            networkTransforms[hash] = netTransform;
            netTransform.prefab = prefab;
            netTransform.hash = hash;

            GigaNetServerGlobals.PublishMessage(new NetInstantiateMessage(prefab, instantiatedNetObject.transform.position,
                hash), DatagramType.Instantiate);

            ServerNetTransform[] childNetTransforms = netTransform.GetComponentsInChildren<ServerNetTransform>(includeInactive: true);
            foreach (ServerNetTransform childNetTransform in childNetTransforms)
            {
                if (childNetTransform == netTransform) continue;

                childNetTransform.hash = NextHash();
                networkTransforms[childNetTransform.hash] = childNetTransform;
            }

            return instantiatedNetObject;
        }

        public int hash;        
        public string prefab;
        public NetworkMovement networkMovement = NetworkMovement.Interpolated;

        private TrackedValue<Vector3> position;
        private TrackedValue<Vector3> rotation;

        private void Start()
        {
            position = new TrackedValue<Vector3>(transform.position);
            rotation = new TrackedValue<Vector3>(transform.eulerAngles);

            if (networkMovement == NetworkMovement.Interpolated)
            {
                GigaNetServerGlobals.interpolationManager.Add(this);
            }
        }

        private void Update()
        {
            position.Track(transform.position);
            rotation.Track(transform.eulerAngles);

            if (networkMovement == NetworkMovement.OnDemand)
            {
                MoveOnDemand();
            }
        }

        private void MoveOnDemand()
        {
            if (position.Changed() || rotation.Changed())
            {
                GigaNetServerGlobals.PublishMessage(new UpdateNetTransformMessage(transform.position, transform.eulerAngles,
                    NetworkMovement.OnDemand, hash), DatagramType.Transform);
            }
        }
    }
}
