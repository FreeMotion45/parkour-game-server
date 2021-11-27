using Assets.Scripts.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityMultiplayer.Shared.Networking;
using UnityMultiplayer.Shared.Networking.Datagrams;

namespace Assets.Scripts.ServerLogic
{
    class PlayerDatabase : MonoBehaviour
    {
        public static readonly Dictionary<BaseNetworkChannel, GameObject> players = new Dictionary<BaseNetworkChannel, GameObject>();

        public static void Publish(object data,
            DatagramType datagramType,
            NetworkChannel sender = null,
            TransportType transport = TransportType.Reliable)
        {
            foreach (NetworkChannel channel in players.Keys)
            {
                if (sender == channel) continue;
                channel.Send(data, datagramType, transport);
            }
        }

        public static string GetName(BaseNetworkChannel channel)
        {
            return players[channel].name;
        }

        public static Vector3 GetPosition(BaseNetworkChannel channel)
        {
            return players[channel].transform.position;
        }

        public static Vector3 GetRotation(BaseNetworkChannel channel)
        {
            return players[channel].transform.eulerAngles;
        }

        public static Transform GetTransform(BaseNetworkChannel channel)
        {
            return players[channel].transform;
        }

        public static Transform[] GetAllTransforms()
        {
            return players.Values.Select(obj => obj.transform).ToArray();
        }
        
        public static GameObject GetPlayerObject(BaseNetworkChannel channel)
        {
            return GetTransform(channel).gameObject;
        }
        
        public static T GetComponent<T>(BaseNetworkChannel channel)
        {
            return players[channel].GetComponent<T>();
        }

        public void OnDisconnect(BaseNetworkChannel disconnectedChannel)
        {
            if (players.ContainsKey(disconnectedChannel))
                players.Remove(disconnectedChannel);
        }
    }
}
