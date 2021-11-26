using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMultiplayer.Shared.Networking.Datagrams
{
    public enum DatagramType
    {
        Transform,
        AbsoluteTransform,
        Instantiate,
        UnreliableKeepAlive,
        Inputs,
    }
}
