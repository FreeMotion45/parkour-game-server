using Assets.Scripts.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityMultiplayer.Shared.Networking;

namespace Assets.Scripts.Shared
{
    class Ownership
    {
        public readonly static Dictionary<BaseNetworkChannel, HashSet<int>> channelTransforms = new Dictionary<BaseNetworkChannel, HashSet<int>>();

        public static bool Owns(BaseNetworkChannel channel, int hash)
        {
            if (!channelTransforms.ContainsKey(channel))
                channelTransforms[channel] = new HashSet<int>();

            return channelTransforms[channel].Contains(hash);
        }

        public static void AddOwned(BaseNetworkChannel channel, int hash)
        {
            if (!channelTransforms.ContainsKey(channel))
                channelTransforms[channel] = new HashSet<int>();

            channelTransforms[channel].Add(hash);
        }

        public static void RemoveOwnership(BaseNetworkChannel channel, int hash)
        {
            if (!channelTransforms.ContainsKey(channel))
                channelTransforms[channel] = new HashSet<int>();

            channelTransforms[channel].Remove(hash);
        }
    }
}
